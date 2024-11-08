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


            //using (var httpClient = new HttpClient())
            //{
            //    // 设置授权头，使用获取的访问令牌
            //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            //    // 构建请求体
            //    var requestBody = new
            //    {
            //        rawDocument = new
            //        {
            //            content = Convert.ToBase64String(fileContent),  // 将文件内容转换为 Base64 编码
            //            mimeType = mimeType  // 文档类型
            //        }
            //    };

            //    // 将请求体序列化为 JSON
            //    var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            //    // 发送 POST 请求到 Document AI API
            //    var response = await httpClient.PostAsync(endpoint, jsonContent);
            //    response.EnsureSuccessStatusCode();  // 确保请求成功
            //    // 读取并输出 API 响应
            //    string responseBody = await response.Content.ReadAsStringAsync();
            //    Console.WriteLine("Document processing completed.");
            //    Console.WriteLine($"Response: {responseBody}");
            //}


            string token = "ya29.a0AcM612wVDPY-6CLG0B-7TCE_9wcKOnbSiMQ7Jr18ReiS2mklHqjKEu0oZdH9TKETOVFNAPuJXWrat8fWHd5VBXKzNJiyNj_Ev2tTRgrdWInMSA661zUhUMaR1_iPy1JP-dZmYuhLuoWguBRgoMjxVGokyUQ0KRX5V4Ydox15zAaCgYKAakSARMSFQHGX2MisCq_rrAPo9blHEAEZmw3Lg0177";



            // 使用访问令牌创建 GoogleCredential 对象
            var credential = GoogleCredential.FromAccessToken(token);

            // 使用凭证创建 DocumentProcessorServiceClient
            var client = new DocumentProcessorServiceClientBuilder
            {
                // 使用 OAuth 令牌认证
                Credential = credential
            }.Build();

            // 创建 ProcessRequest 请求
            var request = new ProcessRequest
            {
                Name = "projects/1018868929654/locations/us/processors/4be69b10501fcd9f:process",
                RawDocument = new RawDocument
                {
                    Content = Google.Protobuf.ByteString.CopyFrom(System.IO.File.ReadAllBytes(localPath)),
                    MimeType = mimeType
                }
            };
            // 增加超时设置
            var callSettings = CallSettings.FromExpiration(Expiration.FromTimeout(TimeSpan.FromMinutes(5)));

            // 调用 ProcessDocument 方法
            var response = await client.ProcessDocumentAsync(request, callSettings);

            // 输出处理结果
            Console.WriteLine(response.Document.Text);




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
    }
}
