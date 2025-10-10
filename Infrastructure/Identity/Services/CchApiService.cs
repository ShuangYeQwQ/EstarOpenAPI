using Application.Interfaces;
using CCH.Pfx.TPI.SDK.ServerSide.BatchProcessing;
using CCH.Pfx.TPI.SDK.ServerSide.Common;
using CCH.Pfx.TPI.SDK.ServerSide.Security;
using CCH.Pfx.TPI.SDK.ServerSide.TaxTransfer;
using Infrastructure.Request.CCHTax;
using Infrastructure.Response.CCHTax;
using Interfaces.Response.CCHTax;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;



namespace Infrastructure.Identity.Services
{
    public class CchApiService: ICchApiService
    {


        public async Task<T> SendRequestAsync<T>(string baseUrl, string accessToken, string endpoint, HttpMethod method, object? requestBody = null) where T : class
        {
            HttpClient _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));  
            JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            HttpRequestMessage request = new HttpRequestMessage(method, endpoint);

            if (requestBody != null)
            {
                string json = JsonSerializer.Serialize(requestBody);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }  

            HttpResponseMessage response = await _client.SendAsync(request);

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<T>(jsonResponse, _jsonOptions) ?? throw new Exception("Failed to deserialize response");
            }
            else
            {
                throw new Exception($"Error: {response.StatusCode}, {jsonResponse}");
            }
        }

        public async void SendTaxReturnInformatioin()
        {
            string baseUrl = "https://your-api-endpoint";
            string accessToken = "your-access-token";
            // CchApiService apiClient = new CchApiService(baseUrl, accessToken); 

            // 传入 `BatchItemGuid`
            string batchItemGuid = "";
            string endpoint = $"api/v1/BatchItemLog?$filter=BatchItemGuid eq '{batchItemGuid}'";

            try
            {
                //var logResult = await apiClient.SendRequestAsync<BatchItemLogResponse>(baseUrl,accessToken,endpoint, HttpMethod.Get);
                //Console.WriteLine($"Batch Log: {logResult.LogDetails}");
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"请求失败: {ex.Message}");
            }
        }

        #region 文件

        /// <summary>
        /// 检索给定批次指南的批次项目列表和生成的可供下载的文件
        /// </summary>
        public async void BatchOutputFiles(string batchGuid)
        {
            try
            {
                string accessToken = "your-access-token";
                string endpoint = $"/api/v1/BatchOutputFiles?$filter=BatchGuid eq '{batchGuid}' ";
                CchApiService apiClient = new CchApiService();
                var logResult = await apiClient.SendRequestAsync<BatchOutputFilesResponse>("https://api.cchaxcess.com/taxservices/oiptax", accessToken, endpoint, HttpMethod.Get);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 流式传输所请求的文件
        /// </summary>
        public async void BatchOutputDownloadFile(string batchGuid,string batchItemGuid,string fileName)
        {
            try
            {
                string accessToken = "your-access-token";
                string endpoint = $"/api/v1/BatchOutputDownloadFile?$filter=BatchGuid eq '{batchGuid}' and BatchItemGuid eq '{batchItemGuid}' and FileName eq '{fileName}' ";
                CchApiService apiClient = new CchApiService();
                var logResult = await apiClient.SendRequestAsync<BatchOutputFilesResponse>("https://api.cchaxcess.com/taxservices/oiptax", accessToken, endpoint, HttpMethod.Get);
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region 客户报税

        /// <summary>
        /// 上传客户报税资料
        /// </summary>
        public void ReturnsImportBatch(string filexml,string configxml)
        {
            try
            {
                AuthenticationTicket ticket = null;
                NamePasswordCredential credential = new NamePasswordCredential("<userName>", "<password>", "<accountNumber>");
                IAuthenticationManager authManager = new AuthenticationManager("https://api.cchaxcess.com", InstallationType.SaaS);
                authManager.Login(credential);
                ticket = authManager.AuthenticationTicket;

                ITaxTransferManager taxTransferManager = new TaxTransferManager(ticket);

                TaxDataImportXmlRequest taxDataRequest = new TaxDataImportXmlRequest();
                taxDataRequest.FilePaths = new List<string> { filexml };
                taxDataRequest.ConfigurationXml = configxml;

                TaxDataOutput taxDataOutput = taxTransferManager.ImportTaxReturnDataXmlAsync(taxDataRequest);

                //Get the file results that contains the validation details
                if (taxDataOutput.FileResults != null)
                {
                    //loop through and get the results
                }
                //Call batch manager to get the batch status based on the Workflow Guid
                IBatchProcessingManager batchProcessingManager = new BatchProcessingManager(ticket);
                BatchResult batchResponse = batchProcessingManager.GetBatchDetail(taxDataOutput.ExecutionId);

            }
            catch (PfxValidationException ex)
            {
                System.Text.StringBuilder errorDetails = new System.Text.StringBuilder();
                foreach (ValidationExceptionDetail exDetail in ex.Details)
                {
                    errorDetails.Append(exDetail.Message);
                }
                Console.WriteLine(errorDetails.ToString());
            }
            catch (PfxApplicationException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 获取批处理状态
        /// </summary>
        /// <param name="batchGuid"></param>
        public void BatchStatus(string batchGuid)
        {
            try
            {
                AuthenticationTicket __ticket = null;
                NamePasswordCredential __credential
                  = new NamePasswordCredential("<userName>", "<password>", "<accountNumber>");
                IAuthenticationManager __authManager
                  = new AuthenticationManager("https://api.cchaxcess.com", InstallationType.SaaS);
                __authManager.Login(__credential);
                __ticket = __authManager.AuthenticationTicket;
                IBatchProcessingManager __batchProcessingManager
                  = new BatchProcessingManager(__ticket);
                if (Guid.TryParse(batchGuid, out Guid myGuid))
                {
                    // 成功转换
                }
                else
                {
                    // 转换失败，guidString 可能不是有效的 GUID
                }
                BatchResult batchResult = __batchProcessingManager.GetBatchDetail(myGuid);
                //Get the batch item guid from the Batch result of GetBatchDetail call
                string batchItemLog = "";
                foreach (BatchItem item in batchResult.Items)
                {
                    if (Guid.TryParse(item.ItemId, out Guid batchguid))
                    {
                        // 成功转换
                    }
                    else
                    {
                        // 转换失败，guidString 可能不是有效的 GUID
                    }
                    batchItemLog = __batchProcessingManager.GetBatchItemLog(batchguid);
                    //Process the Batch Item Log data ....
                }
            }
            catch (PfxValidationException ex)
            {
                System.Text.StringBuilder __errorDetails = new System.Text.StringBuilder();
                foreach (ValidationExceptionDetail exDetail in ex.Details)
                {
                    __errorDetails.Append(exDetail.Message);
                }
                Console.WriteLine(__errorDetails.ToString());
            }
            catch (PfxApplicationException ex)
            {
                Console.WriteLine(ex.Message);
            }




        }
       
        /// <summary>
        /// 提交Return清单以供计算
        /// </summary>
        public async void CalculateReturn(CalculateReturnRequest calculateReturnRequest)
        {
            try
            {
                CchApiService apiClient = new CchApiService();
                var logResult = await apiClient.SendRequestAsync<CalculateReturnResponse>("https://api.cchaxcess.com/taxservices/oiptax", "your-access-token", "/api/v1/CalculateReturn", HttpMethod.Post, calculateReturnRequest);
            }
            catch (Exception ex)
            {
               
            }
        }

        /// <summary>
        /// 检索给定申报表的税务机关列表
        /// </summary>
        public async void ELFReturnUnits(string returnID, string exportCategory)
        {
            try
            {
                string accessToken = "your-access-token";
                string endpoint = $"/api/v1/BatchItemLog?$filter=ReturnID eq '{returnID}' ExportCategory eq '{exportCategory}'";
                CchApiService apiClient = new CchApiService();
                var logResult = await apiClient.SendRequestAsync<ELFReturnUnitsResponse>("https://api.cchaxcess.com/taxservices/oiptax", accessToken, endpoint, HttpMethod.Get);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 将return units上传至电子文件系统并保存在那里
        /// </summary>
        public async void ELFUploadAndHold(ELFUploadAndHoldRequest eLFUploadAndHoldRequest)
        {
            try
            {
                string accessToken = "your-access-token";
                string endpoint = $"/api/v1/ELFUploadAndHold";
                CchApiService apiClient = new CchApiService();
                var logResult = await apiClient.SendRequestAsync<string>("https://api.cchaxcess.com/taxservices/oiptax", accessToken, endpoint, HttpMethod.Post, eLFUploadAndHoldRequest);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 检索上传和保留电子文件请求的状态
        /// </summary>
        public async void ELFUploadStatus(string sessionGuid)
        {
            try
            {
                string accessToken = "your-access-token";
                string endpoint = $"/api/v1/ELFUploadAndHold?$filter=ReturnID eq '{sessionGuid}'";
                CchApiService apiClient = new CchApiService();
                var logResult = await apiClient.SendRequestAsync<ELFUploadStatusResponse>("https://api.cchaxcess.com/taxservices/oiptax", accessToken, endpoint, HttpMethod.Get);
            }
            catch (Exception ex)
            {

            }
        }
       
        /// <summary>
        /// 检索电子文件单位列表及其相应状态
        /// </summary>
        public async void ELFReleaseCandidates(string returnID, string clientGuid, string exportCategory)
        {
            try
            {
                string accessToken = "your-access-token";
                string endpoint = $"/api/v1/ELFReleaseCandidates?$filter=ReturnID eq '{returnID}' and ClientGuid eq '{clientGuid}' and ExportCategory eq '{exportCategory}'";
                CchApiService apiClient = new CchApiService();
                var logResult = await apiClient.SendRequestAsync<ELFReleaseCandidatesResponse>("https://api.cchaxcess.com/taxservices/oiptax", accessToken, endpoint, HttpMethod.Get);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 将电子文件发布候选文件提交至电子文件系统
        /// </summary>
        public async void ELFReleaseReturns(ELFReleaseReturnsRequest eLFReleaseReturnsRequest)
        {
            try
            {
                string accessToken = "your-access-token";
                CchApiService apiClient = new CchApiService();
                var logResult = await apiClient.SendRequestAsync<ELFReleaseReturnsResponse>("https://api.cchaxcess.com/taxservices/oiptax", accessToken, "/api/v1/ELFReleaseReturns", HttpMethod.Post, eLFReleaseReturnsRequest);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 根据客户端列表获取 Elf 状态
        /// </summary>
        public async void ElfStatusClientList(ElfStatusByClientRequest elfStatusByClientRequest)
        {
            try
            {
                string accessToken = "your-access-token";
                CchApiService apiClient = new CchApiService();
                var logResult = await apiClient.SendRequestAsync<ElfStatusByClientResponse>("https://api.cchaxcess.com/api", accessToken, "/ElfStatusService/v1.0/ElfStatus/Client", HttpMethod.Post, elfStatusByClientRequest);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 根据ReturnId获取Elf返回历史详情
        /// </summary>
        /// <param name="calculateReturnRequest"></param>
        public async void ElfStatusHistory(string returnID)
        {
            try
            {
                string accessToken = "your-access-token";
                string endpoint = $"/api/ElfStatusService/v1.0/ElfStatus/History?ReturnId= '{returnID}' ";
                CchApiService apiClient = new CchApiService();
                var logResult = await apiClient.SendRequestAsync<ElfStatusHistoryResponse>("https://api.cchaxcess.com", accessToken, endpoint, HttpMethod.Get);
            }
            catch (Exception ex)
            {

            }
        }


        #endregion
    }
}
