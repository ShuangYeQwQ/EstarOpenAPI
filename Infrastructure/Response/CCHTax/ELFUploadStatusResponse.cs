using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Response.CCHTax
{
    public class ComplianceMessage
    {
        public int MessageIndex { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int MessageOptions { get; set; }
        public int MessageType { get; set; }
    }

    public class ELFUploadStatusResponse
    {
        public bool HasError { get; set; }
        public int PercentageCompleted { get; set; }
        public int ELFUploadStatus { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public List<ComplianceMessage> ComplianceMessages { get; set; } = new List<ComplianceMessage>();
    }
}
