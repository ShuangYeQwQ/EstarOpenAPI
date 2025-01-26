using System;
using System.IO;
using System.Threading.Channels;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using Google.Api.Gax.Grpc;
using Google.Api.Gax;
using Google.Cloud.DocumentAI.V1;
using Microsoft.Identity.Client;
using Grpc.Core;
using Google.Cloud.Storage.V1;
using Google.Apis.Storage.v1.Data;
using Google.Protobuf;
using Google.Cloud.Firestore;
using System.Threading;
using static Google.Apis.Requests.BatchRequest;
using EStarGoogleCloud.Response;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using iText.Kernel.Pdf;
namespace EStarGoogleCloud
{
    public  class DocumentAIRequest
    {
        private static readonly HttpClient client = new HttpClient();
        public static async Task GetDocumentAI() {
            string projectId = "semiotic-art-418621";
            string locationId = "us";
            string processorId = "4be69b10501fcd9f";
            string localPath = "D:\\work\\trainingDocuments\\W2-img(1).png";
            string mimeType = "image/png";


                // 替换为你的 API 密钥
                string apiKey = "ya29.a0AcM612wVDPY-6CLG0B-7TCE_9wcKOnbSiMQ7Jr18ReiS2mklHqjKEu0oZdH9TKETOVFNAPuJXWrat8fWHd5VBXKzNJiyNj_Ev2tTRgrdWInMSA661zUhUMaR1_iPy1JP-dZmYuhLuoWguBRgoMjxVGokyUQ0KRX5V4Ydox15zAaCgYKAakSARMSFQHGX2MisCq_rrAPo9blHEAEZmw3Lg0177";
                string endpoint = "https://us-documentai.googleapis.com/v1/projects/1018868929654/locations/us/processors/4be69b10501fcd9f:process";
                // 确保文件路径存在并且可以读取
                if (!File.Exists(localPath))
                {
                    Console.WriteLine("File not found: " + localPath);
                    return;
                }
                // 读取 PDF 文件内容
                byte[] fileContent = File.ReadAllBytes(localPath);


            using (var httpClient = new HttpClient())
            {
                // 设置授权头，使用获取的访问令牌
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                // 构建请求体
                var requestBody = new
                {
                    rawDocument = new
                    {
                        content = Convert.ToBase64String(fileContent),  // 将文件内容转换为 Base64 编码
                        mimeType = mimeType  // 文档类型
                    }
                };

                // 将请求体序列化为 JSON
                var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                // 发送 POST 请求到 Document AI API
                var response = await httpClient.PostAsync(endpoint, jsonContent);
                response.EnsureSuccessStatusCode();  // 确保请求成功
                // 读取并输出 API 响应
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Document processing completed.");
                Console.WriteLine($"Response: {responseBody}");
            }


            //string token = "ya29.a0AcM612wVDPY-6CLG0B-7TCE_9wcKOnbSiMQ7Jr18ReiS2mklHqjKEu0oZdH9TKETOVFNAPuJXWrat8fWHd5VBXKzNJiyNj_Ev2tTRgrdWInMSA661zUhUMaR1_iPy1JP-dZmYuhLuoWguBRgoMjxVGokyUQ0KRX5V4Ydox15zAaCgYKAakSARMSFQHGX2MisCq_rrAPo9blHEAEZmw3Lg0177";

            //// 使用访问令牌创建 GoogleCredential 对象
            //var credential = GoogleCredential.FromAccessToken(token);

            //// 使用凭证创建 DocumentProcessorServiceClient
            //var client = new DocumentProcessorServiceClientBuilder
            //{
            //    // 使用 OAuth 令牌认证
            //    Credential = credential
            //}.Build();

            //// 创建 ProcessRequest 请求
            //var request = new ProcessRequest
            //{
            //    Name = "projects/1018868929654/locations/us/processors/4be69b10501fcd9f:process",
            //    RawDocument = new RawDocument
            //    {
            //        Content = Google.Protobuf.ByteString.CopyFrom(System.IO.File.ReadAllBytes(localPath)),
            //        MimeType = mimeType
            //    }
            //};
            //// 增加超时设置
            //var callSettings = CallSettings.FromExpiration(Expiration.FromTimeout(TimeSpan.FromMinutes(5)));

            //// 调用 ProcessDocument 方法
            //var response = await client.ProcessDocumentAsync(request, callSettings);

            //// 输出处理结果
            //Console.WriteLine(response.Document.Text);




            //var client = new DocumentProcessorServiceClientBuilder
            //{
            //    Endpoint = $"{locationId}-documentai.googleapis.com"
            //}.Build();


            //using var fileStream = File.OpenRead(localPath);
            //var rawDocument = new RawDocument
            //{
            //    Content = ByteString.FromStream(fileStream),
            //    MimeType = mimeType
            //};
            ////var Contents = rawDocument.Content.ToBase64();
            //// Initialize request argument(s)
            //var request = new ProcessRequest
            //{
            //    Name = ProcessorName.FromProjectLocationProcessor(projectId, locationId, processorId).ToString(),
            //    RawDocument = rawDocument
            //};

            //// Make the request
            //var response = client.ProcessDocument(request);

            //var document = response.Document;
            //Console.WriteLine(document.Text);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderPath">文件位置</param>
        /// <param name="type">处理器类型，0：获取表格名字，1：处理表格</param>
        /// <returns></returns>
        public static async Task<List<DocumentAIResponse>> GetDocumentAIFromGCSParallel(string endpoint, string folderPath)
        {
            string bucketName = "estar_bucket";  // 你的存储桶名称
            endpoint = "https://us-documentai.googleapis.com/v1"+endpoint;//处理器
            string jsonKeyFilePath = "C:\\work\\EstarOpenAPI\\EstarOpenAPI\\File\\semiotic-art-418621-88496cd79e0d.json";  // 替换为你的密钥文件路径

            // 获取访问令牌
            var token = await GetAccessTokenAsync(jsonKeyFilePath);

            // 获取文件列表
            var objects = await GetGCSObjectsAsync(jsonKeyFilePath, bucketName, folderPath);
            List<DocumentAIResponse> documentAIResponses = new List<DocumentAIResponse>();
            // 处理每个文件
            foreach (var storageObject in objects)
            {
                string filePath = storageObject.Name;
                string filetype = storageObject.ContentType;
                // 调用处理文档的方法
                var documentAIResponse = await ProcessDocumentAsync( filePath, filetype, token, endpoint);
                documentAIResponses.Add(documentAIResponse);
            }
            return documentAIResponses;
        }


        public static async Task GetDocumentAIFrom(string folderPath)
        {
            string projectId = "1018868929654";
            string locationId = "us";
            string processorId = "6cf08698c864bb11";
            string bucketName = "estar_bucket";  // 你的存储桶名称
            string endpoint = "https://us-documentai.googleapis.com/v1/projects/1018868929654/locations/us/processors/6cf08698c864bb11:process";
            string jsonKeyFilePath = "C:\\work\\EstarOpenAPI\\EstarOpenAPI\\File\\semiotic-art-418621-88496cd79e0d.json";  // 替换为你的密钥文件路径

            // 获取访问令牌
            var token = await GetAccessTokenAsync(jsonKeyFilePath);

            // 获取文件列表
            var objects = await GetGCSObjectsAsync(jsonKeyFilePath, bucketName, folderPath);
            // 创建一个 Task 列表来存储每个文件的异步处理任务
            List<Task> tasks = new List<Task>();
            int maxDegreeOfParallelism = 5;  // 控制最大并发任务数
            SemaphoreSlim semaphore = new SemaphoreSlim(maxDegreeOfParallelism);
            // 处理每个文件
            foreach (var storageObject in objects)
            {
                string filePath = storageObject.Name;
                string filetype = storageObject.ContentType;
                tasks.Add(Task.Run(async () =>
                {
                    // 等待可用的信号量
                    await semaphore.WaitAsync();
                    try
                    {
                        // 调用处理文档的方法
                        await ProcessDocumentAsync(filePath, filetype, token, endpoint);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing document {filePath}: {ex.Message}");
                    }
                    finally
                    {
                        // 释放信号量
                        semaphore.Release();
                    }
                }));
            }
            // 等待所有任务完成
            await Task.WhenAll(tasks);
        }
        
        public static async Task<string> GetAccessTokenAsync(string jsonKeyFilePath)
        {
            var credential = GoogleCredential.FromFile(jsonKeyFilePath);
            credential = credential.CreateScoped("https://www.googleapis.com/auth/cloud-platform");
            var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            return token;
        }
        
        public static async Task<List<Google.Apis.Storage.v1.Data.Object>> GetGCSObjectsAsync(string jsonKeyFilePath, string bucketName, string folderPath)
        {
            var storageClient = StorageClient.Create(GoogleCredential.FromFile(jsonKeyFilePath));
            var objects = new List<Google.Apis.Storage.v1.Data.Object>();
            await foreach (var storageObject in storageClient.ListObjectsAsync(bucketName, folderPath))
            {
                objects.Add(storageObject);
            }
            return objects;
        }


        // 从 MediaLink 下载 PDF 文件内容
        public static async Task<byte[]> DownloadPdfFromMediaLink(string mediaLink)
        {
            using (HttpClient client = new HttpClient())
            {
                // 发送请求获取文件内容
                HttpResponseMessage response = await client.GetAsync(mediaLink);
                response.EnsureSuccessStatusCode();

                // 获取响应内容
                byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                Console.WriteLine($"PDF 文件已下载，大小: {fileBytes.Length} bytes");
                return fileBytes;
            }
        }
        // 按页拆分 PDF 文件并返回每页的 byte[]
        public static List<byte[]> SplitPdfByPageToByteArray(byte[] pdfBytes)
        {
            List<byte[]> pdfPages = new List<byte[]>();

            // 使用 PdfReader 加载 byte[] 中的 PDF
            using (MemoryStream inputStream = new MemoryStream(pdfBytes))
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(inputStream)))
            {
                int totalPages = pdfDoc.GetNumberOfPages();

                // 按页拆分 PDF 文件
                for (int i = 1; i <= totalPages; i++)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        // 创建新的 PDF 文档，将当前页写入到 MemoryStream
                        using (PdfDocument newPdfDoc = new PdfDocument(new PdfWriter(memoryStream)))
                        {
                            // 复制当前页到新的 PDF 文档
                            pdfDoc.CopyPagesTo(i, i, newPdfDoc);
                        }

                        // 将拆分后的页面的 byte[] 添加到列表
                        pdfPages.Add(memoryStream.ToArray());
                    }
                }
            }

            return pdfPages;
        }
        //文档名称
        public static async Task<DocumentAIResponse> ProcessDocumentNameAsync(byte[] fileContent, string fileType, string token, string endpoint)
        {
            DocumentAIResponse documentAIResponse = new DocumentAIResponse();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // 设置授权头，使用获取的访问令牌
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // 构建请求体
                    var requestBody = new
                    {
                        rawDocument = new
                        {
                            content = Convert.ToBase64String(fileContent),  // 将文件内容转换为 Base64 编码
                            mimeType = fileType  // 文档类型
                        }
                    };
                    // 将请求体序列化为 JSON
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                    // 发送 POST 请求到 Document AI API
                    var response = await httpClient.PostAsync(endpoint, jsonContent);
                    response.EnsureSuccessStatusCode();  // 确保请求成功
                                                         // 读取并输出 API 响应
                    string responseBody = await response.Content.ReadAsStringAsync();
                    documentAIResponse = JsonConvert.DeserializeObject<DocumentAIResponse>(responseBody);
                }
                return documentAIResponse;
            }
            catch (HttpRequestException ex)
            {
                GoogleSqlDBHelper.ExecuteNonQuery("使用GoogleDocumentAi失败：" + ex.Message, "");
                return documentAIResponse;
            }
        }

        //文档处理
        public static async Task<DocumentAIResponse> ProcessDocumentAsync( string filePath, string fileType, string token,string endpoint)
        {
            DocumentAIResponse documentAIResponse = new DocumentAIResponse();
            try
            {
                var request = new 
                {
                    gcs_document = new 
                    {
                        gcs_uri = $"gs://{filePath}",
                        mime_type = fileType  // 根据需要调整 MIME 类型
                    }
                };
                // 将请求体序列化为 JSON
                var jsonContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                // 设置授权头，使用获取的访问令牌
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // 发送 POST 请求到 Document AI API
                var response = await client.PostAsync(endpoint, jsonContent);
                response.EnsureSuccessStatusCode();  // 确保请求成功

                // 读取并输出 API 响应
                string responseBody = await response.Content.ReadAsStringAsync();
                documentAIResponse = JsonConvert.DeserializeObject<DocumentAIResponse>(responseBody);
                return documentAIResponse;
            }
            catch (HttpRequestException ex)
            {
                GoogleSqlDBHelper.ExecuteNonQuery("使用GoogleDocumentAi失败：" + ex.Message, "");
                return documentAIResponse;
            }
        }
        
        
    }
}
