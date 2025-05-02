using Application.Interfaces;
using Azure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Identity.Services
{
    public class CchApiService: ICchApiService
    {
        public CchApiService()
        {
          
        }


        public async Task<T> SendRequestAsync<T>(string baseUrl, string accessToken,  string endpoint, HttpMethod method, object? requestBody = null) where T : class
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
                return JsonSerializer.Deserialize<T>(jsonResponse, _jsonOptions) ?? throw new Exception("Failed to deserialize response.");
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
            string batchItemGuid = "12345-abcde-67890";
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
    }
}
