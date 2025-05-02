using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Infrastructure.Request.CCHTax
{
    // 定义 XML 结构
    [XmlRoot("TaxCalculateReturnOptions")]
    public class CalculateReturnRequest
    {
        public string TaxReturnClientDataOptions { get; set; }

        public string TaxReturnCalcAuthorizationOptions { get; set; }
    }
}
