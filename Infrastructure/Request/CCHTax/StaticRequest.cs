using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Infrastructure.Request.CCHTax
{
    public class CchStaticRequest
    {
        public List<string> ReturnId { get; set; } // 税表 ID 列表

        public string ConfigurationXml { get; set; } // XML 格式的导出选项
    }
}
