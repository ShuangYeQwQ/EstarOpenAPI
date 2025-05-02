using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Infrastructure.Request.CCHTax
{
    public class DeleteReturnRequest
    {
        [JsonPropertyName("ReturnId")]
        public List<string> ReturnId { get; set; }

        [JsonPropertyName("RecoveryCopies")]
        public bool? RecoveryCopies { get; set; }  // 可选参数，可能为空

        [JsonPropertyName("K1ImportFiles")]
        public bool? K1ImportFiles { get; set; }  // 可选参数，可能为空
    }
}
