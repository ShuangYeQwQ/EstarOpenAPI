using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Response.CCHTax
{
    public class ReturnLatestHistoryResponse
    {
        public string Activity { get; set; } = string.Empty;
        public string ActivityCode { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public DateTime LastRecordedDateTime { get; set; }
        public bool ReturnFound { get; set; }
        public string ReturnId { get; set; } = string.Empty;
        public string StaffName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
