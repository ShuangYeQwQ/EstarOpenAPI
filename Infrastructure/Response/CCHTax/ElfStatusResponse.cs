using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Response.CCHTax
{
    public class ElfStatusResponse
    {
        public int ElfSDKStatus { get; set; }
        public List<ElfStatusDetail> ElfStatusList { get; set; } = new List<ElfStatusDetail>();
        public string Error { get; set; } = string.Empty;
    }

    public class ElfStatusDetail
    {
        public string ReturnGuid { get; set; } = string.Empty;
        public string ReturnId { get; set; } = string.Empty;
        public string StatusDate { get; set; } = string.Empty;
        public string SummaryStatus { get; set; } = string.Empty;
        public string CategoryofReturn { get; set; } = string.Empty;
        public string TypeOfReturn { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string UnitType { get; set; } = string.Empty;
    }
}
