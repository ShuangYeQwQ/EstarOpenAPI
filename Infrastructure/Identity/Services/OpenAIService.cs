using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Embeddings;
namespace Infrastructure.Identity.Services
{
    

    public class OpenAIService
    {
        private readonly EmbeddingClient _embeddingClient;
        private readonly ChatClient _chatClient;

        public OpenAIService(string apiKey)
        {
            _embeddingClient = new EmbeddingClient("text-embedding-3-small", apiKey);
            _chatClient = new ChatClient("gpt-4o-mini", apiKey);
        }

        // 批量生成 embedding
        public  async Task<List<float[]>> GetEmbeddingsBatchAsync(IEnumerable<string> texts, int batchSize = 32)
        {
            var all = texts.ToList();
            var results = new List<float[]>();

            for (int i = 0; i < all.Count; i += batchSize)
            {
                var slice = all.Skip(i).Take(batchSize);
                foreach (var t in slice)
                {
                    OpenAIEmbedding embedding = _embeddingClient.GenerateEmbedding(t);
                    results.Add(embedding.ToFloats().ToArray());
                }
            }
            return results;
        }

        // 基于上下文生成回答
        public async Task<string> GetAnswerFromContextAsync(string question, IEnumerable<string>? contextPieces)
        {
            var system = "";
            var userPrompt = "";
            if (contextPieces == null) {
                system = "你是一个专业的文档助手，基于用户的内容回复文档内容。\r\n        请遵循以下规则：\r\n          1. 只基于提供的文档内容回答，不要编造信息\r\n                  2. 回答要准确、简洁、专业\r\n ";
                userPrompt = $"上下文：\n{question}\n\n请基于上下文回复该文本主要内容是什么,以及需要注意什么。";
            }
            else
            {
                var context = string.Join("\n---\n", contextPieces);
                system = "你是一个专业的文档助手，基于用户提供的文档内容回答问题。\r\n                    请遵循以下规则：\r\n                    1. 只基于提供的文档内容回答，不要编造信息\r\n                    2. 如果文档中没有相关信息，请如实告知\r\n                    3. 回答要准确、简洁、专业\r\n                    4. 可以引用文档中的具体内容来支持你的回答\r\n                    5. 如果问题与文档内容无关，请礼貌地说明";
                userPrompt = $"上下文：\n{context}\n\n问题：{question}\n\n请基于上下文回答，并指出相关片段来源。";

            }

            var messages = new List<ChatMessage>
        {
            new SystemChatMessage(system),
            new UserChatMessage(userPrompt)
        };

            ChatCompletion completion = await _chatClient.CompleteChatAsync(messages);

            var answer = completion.Content[0].Text;

            return answer ?? "未能生成回答";
        }
    }
}


