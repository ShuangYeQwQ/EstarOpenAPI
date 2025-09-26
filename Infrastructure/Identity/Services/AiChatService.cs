using Application.Interfaces;
using Infrastructure.Identity.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using static Infrastructure.Identity.Services.AiChatService;
using PaypalServerSdk.Standard.Models;
using static Google.Cloud.DocumentAI.V1.Document.Types.Page.Types;
using System.Text.Json;
using System.Collections;
using GoogleCloudModel;
using Application.Wrappers;
using Application.RequestModel;
using Microsoft.AspNetCore.Http;
using Stripe;
using Infrastructure.Shared;

namespace Infrastructure.Identity.Services
{
    public class AiChatService
    {
        public class AIChatService : IAiChatService
        {

            //        public async Task<string> GetResponse(string userMessage, string? context = null)
            //        {
            //            string apiKey = _config["OpenAI:ApiKey"];
            //            var api = new OpenAIClient(apiKey);

            //            string prompt = context != null
            //                ? $"文件内容:\n{context}\n\n用户问题: {userMessage}"
            //                : userMessage;

            //            var request = new CompletionRequest
            //            {
            //                Prompt = prompt,
            //                Model = "gpt-3.5-turbo-instruct", // 这里可以调整为更合适的模型，比如 GPT-4
            //                MaxTokens = 500,
            //                Temperature = 0.7
            //            };

            //            try
            //            {
            //                var result = await api.CompletionsEndpoint.CreateCompletionAsync(request);
            //                return result.ToString();
            //            }
            //            catch (Exception ex)
            //            {
            //                return $"AI 服务错误: {ex.Message}";
            //            }
            //        }



            private readonly HttpClient _httpClient;
            //https://in03-45c7c25d7b6c39b.serverless.gcp-us-west1.cloud.zilliz.com/v2/vectordb
            //9390036a8ed963044e33008912e3b74e8067a688999d873f09a582a809f3c7077c8b8247e64cfaac3e755f8b86da547b5b565aa1
            public AIChatService(string apiKey)
            {
                _httpClient = new HttpClient();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            private async Task<string> PostAsync(string url, string jsonContent)
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://in03-45c7c25d7b6c39b.serverless.gcp-us-west1.cloud.zilliz.com/v2/vectordb" + url)
                {
                    Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
                };

                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            /// <summary>
            /// 创建集合
            /// </summary>
            /// <param name="name"></param>
            /// <param name="dimension"></param>
            /// <returns></returns>
            public Task<string> CreateCollectionAsync(string userid, int dimension)
            {
                string payload = $@"{{
            ""collectionName"": ""{userid}"",
            ""dimension"": 1536,
            ""metricType"": ""COSINE"",
            ""vectorField"": ""vector""
        }}";
                return PostAsync("/collections/create", payload);
            }
            /// <summary>
            /// 创建数据
            /// </summary>
            /// <param name="collectionName"></param>
            /// <param name="vectors"></param>
            /// <param name="texts"></param>
            /// <returns></returns>
            public async Task<string> InsertVectorAsync(string collectionName,string filename,string id, float[][] vectors, string[] texts)
            {
                if (vectors.Length != texts.Length)
                    throw new ArgumentException("vectors 和 texts 数量必须一致");

                var dataList = new List<object>();
                var zillizid = id;
                for (int i = 0; i < vectors.Length; i++)
                {
                    dataList.Add(new
                    {
                        id = zillizid + i, // 可以换成 Guid 或数据库自增
                        vector = vectors[i],
                        text = texts[i]
                    });
                }
                zillizid = zillizid + vectors.Length;
                var payload = new
                {
                    collectionName = collectionName,
                    data = dataList
                };

                string json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                });

                return await PostAsync("/entities/insert", json);
            }
            ///// <summary>
            ///// 更新数据
            ///// </summary>
            ///// <param name="collectionName"></param>
            ///// <param name="userId"></param>
            ///// <param name="filename"></param>
            ///// <param name="vectors"></param>
            ///// <param name="texts"></param>
            ///// <returns></returns>
            ///// <exception cref="ArgumentException"></exception>
            //public async Task<string> UpsertVectorAsync(string collectionName, string userId, string filename, float[][] vectors, string[] texts)
            //{
            //    if (vectors.Length != texts.Length)
            //        throw new ArgumentException("vectors 和 texts 数量必须一致");

            //    var dataList = new List<object>();

            //    for (int i = 0; i < vectors.Length; i++)
            //    {
            //        dataList.Add(new
            //        {
            //            id = i + 1, // 可以换成 Guid 或数据库自增
            //            vector = vectors[i],
            //            userId = userId,
            //            text = texts[i],
            //            filename = filename
            //        });
            //    }

            //    var payload = new
            //    {
            //        collectionName = collectionName,
            //        data = dataList
            //    };

            //    string json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            //    {
            //        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //        WriteIndented = false
            //    });

            //    return await PostAsync("/entities/upsert", json);
            //}
            /// <summary>
            /// 搜索用户下全部数据
            /// </summary>
            /// <param name="collectionName"></param>
            /// <param name="userId"></param>
            /// <returns></returns>
            public async Task<string> SearchVectorAsync(string collectionName, string userId) {
                string json = $@"{{ ""collectionName"": ""{collectionName}"",""filter"": ""userId == \""{userId}\""}}";
                return await PostAsync("/entities/query", json);
            }
            /// <summary>
            /// 删除用户数据
            /// </summary>
            /// <param name="collectionName">userid</param>
            /// <param name="id">dataid</param>
            /// <returns></returns>
            public async Task<string> DeleteVectorAsync(string collectionName, string id,string filename)
            {
                string json = $@"{{ ""collectionName"": ""{collectionName}"",""filter"": ""id == \""{id}\""}}";
                return await PostAsync("/entities/delete", json);
            }

            /// <summary>
            /// 文件提取文本，保存向量数据
            /// </summary>
            /// <param name="signup_req"></param>
            /// <returns></returns>
            public async Task<Response<string>> UploadDocument(common_req<IFormFile> signup_req)
            {
                // 1. 提取文本 (伪代码)
                var text = await FileHelper.ExtractTextAsync(signup_req.Actioninfo);

                // 2. 向量化
                OpenAIService openAIService = new OpenAIService("");
                var embeddings = await openAIService.GetEmbeddingsBatchAsync(new[] { text });

                // 3. 存储
                await InsertVectorAsync(signup_req.User, signup_req.Actioninfo.FileName,"", embeddings.ToArray(), new[] { text });

                return new Response<string>("文件已处理并存入向量数据库", "");
            }
            /// <summary>
            /// 根据用户文件获取关联回答
            /// </summary>
            /// <param name="signup_req"></param>
            /// <returns></returns>
            public async Task<Response<string>> UserChat(common_req<IFormFile> signup_req)
            {
                // 1. 提取文本 (伪代码)
                var text = await FileHelper.ExtractTextAsync(signup_req.Actioninfo);
                // 2.  query embedding
                OpenAIService openAIService = new OpenAIService("");
                var queryEmbedding = (await openAIService.GetEmbeddingsBatchAsync(new[] { text }))[0];

                // 3. 检索
                var searchResultJson = await SearchVectorAsync(signup_req.User, queryEmbedding);
                var searchResult = JsonDocument.Parse(searchResultJson)
                    .RootElement.GetProperty("data")
                    .EnumerateArray()
                    .Select(e => e.GetProperty("text").GetString())
                    .ToList();

                // 4. 基于上下文生成答案
                var answer = await openAIService.GetAnswerFromContextAsync(text, searchResult);

                return new Response<string>(answer, "");
            }



        }
    }


}
