using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Infrastructure.Request.CCHTax
{
    public class ElfReleaseReturnsRequest
    {
        public string ReturnUnitKeyGuid { get; set; }  // 这里是 string

        public string UploadHistoryGuid { get; set; }

        public int UploadHistoryKey { get; set; }
    }
}
