using Infrastructure.Identity.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Infrastructure.Identity.Services
{
    //public class AiChatService
    //{
    //    public class AIService
    //    {
    //        private readonly IConfiguration _config;

    //        public AIService(IConfiguration config)
    //        {
    //            _config = config;
    //        }

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

    //    }
    //}


}
