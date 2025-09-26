using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseModel.Chat
{
    public class AiChat_res
    {
        public string Answer { get; set; }
        public List<DocumentChunk> RelevantChunks { get; set; } = new();
        public string SessionId { get; set; }

        public class DocumentChunk
        {
            public string Id { get; set; } = Guid.NewGuid().ToString();
            public string CustomerId { get; set; }
            public string FileName { get; set; }
            public string Content { get; set; }
            public float[] Embedding { get; set; }
            public int ChunkIndex { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }
    }
}
