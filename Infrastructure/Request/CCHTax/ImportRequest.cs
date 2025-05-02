using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Infrastructure.Request.CCHTax
{
    public class ImportRequest
    {
        public List<string> FileDataList { get; set; }  // 文件数据的字节数组列表
        public string ConfigurationXml { get; set; }   // XML 配置选项字符串
    }
    // 定义 XML 结构
    [XmlRoot("TaxDataExportOptions")]
    public class TaxDataImportOptions
    {
        public string ImportMode { get; set; }
        public bool CaseSensitiveMatching { get; set; }
        public string InvalidContentErrorHandling { get; set; }
        public bool CalcReturnAfterImport { get; set; }
    }
    }
