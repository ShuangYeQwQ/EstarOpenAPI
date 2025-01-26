using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EStarGoogleCloud.Response
{
    public class DocumentAIResponse
    {
        public GoogleDocumentAI document { get; set; }
    }

    public class GoogleDocumentAI
    {
        public string text { get; set; }
        public List<Entity> entities { get; set; }
    }

    public class Entity
    {
        public string type { get; set; }
        public string mentionText { get; set; }
        public float confidence { get; set; }
        public TextAnchor textAnchor { get; set; }
    }
    public class TextAnchor
    {
        public List<TextSegment> textSegments { get; set; }
    }

    public class TextSegment
    {
        public int startIndex { get; set; }
        public int endIndex { get; set; }
    }
}
