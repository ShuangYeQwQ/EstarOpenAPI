using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Infrastructure.Request.CCHTax
{
    [XmlRoot("TaxReturnRollforwardOptions")]
    public class RollForwardBatchRequest
    {
        public string ClientDataOption { get; set; }
        public bool RollForwardLists { get; set; }
        public bool IncludeAllPriorYearAmounts { get; set; }
        public bool RollForwardReturnsSuppressedOnPriorYear { get; set; }
        public bool SwitchReturnTypeForCorpAndSCorp { get; set; }
    }
}
