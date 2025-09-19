using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Request.CchWorkSheet
{
    
        public class ReturnHeaderInfo
        {
            public string ClientID { get; set; }
            public string TaxYear { get; set; }
            public string ReturnType { get; set; }
            public string ReturnGroupName { get; set; }
            public string Country { get; set; }
            public string OfficeName { get; set; }
            public string BusinessUnitName { get; set; }
            public string ConfigurationSet { get; set; }
            public string ReturnVersion { get; set; }
            public string EINorSSN { get; set; }
            public string ControlNumber { get; set; }
        }
    public class TaxPayerDetailsInfo
    {
        public string NameLine1 { get; set; }
        public string NameLine2 { get; set; }
    }
    public class PayloadGenerator { 
       

        public string GenerateFullPayload(
            ReturnHeaderInfo headerInfo,
            TaxPayerDetailsInfo payerDetails,
           List<string> xmlForm)
        {
            // 生成1099-INT表格XML
            var intGenerator = new Interest1099_INTGenerator();

            // 构建完整的Payload XML
            StringBuilder xmlBuilder = new StringBuilder();

            xmlBuilder.AppendLine(@"<Payload DataType=""Tax"" DataFormat=""Standard"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">");
            xmlBuilder.AppendLine("  <TaxReturn>");

            // 添加ReturnHeader
            xmlBuilder.AppendLine("    <ReturnHeader");
            xmlBuilder.AppendLine($"      ClientID=\"{EscapeXml(headerInfo.ClientID)}\"");
            xmlBuilder.AppendLine($"      TaxYear=\"{EscapeXml(headerInfo.TaxYear)}\"");
            xmlBuilder.AppendLine($"      ReturnType=\"{EscapeXml(headerInfo.ReturnType)}\"");
            xmlBuilder.AppendLine($"      ReturnGroupName=\"{EscapeXml(headerInfo.ReturnGroupName)}\"");
            xmlBuilder.AppendLine($"      Country=\"{EscapeXml(headerInfo.Country)}\"");
            xmlBuilder.AppendLine($"      OfficeName=\"{EscapeXml(headerInfo.OfficeName)}\"");
            xmlBuilder.AppendLine($"      BusinessUnitName=\"{EscapeXml(headerInfo.BusinessUnitName)}\"");
            xmlBuilder.AppendLine($"      ConfigurationSet=\"{EscapeXml(headerInfo.ConfigurationSet)}\"");
            xmlBuilder.AppendLine($"      ReturnVersion=\"{EscapeXml(headerInfo.ReturnVersion)}\"");
            xmlBuilder.AppendLine($"      EINorSSN=\"{EscapeXml(headerInfo.EINorSSN)}\"");
            xmlBuilder.AppendLine($"      ControlNumber=\"{EscapeXml(headerInfo.ControlNumber)}\"");
            xmlBuilder.AppendLine("    />");

            // 添加TaxPayerDetails
            xmlBuilder.AppendLine($"    <TaxPayerDetails NameLine1=\"{EscapeXml(payerDetails.NameLine1)}\" NameLine2=\"{EscapeXml(payerDetails.NameLine2)}\" />");

            // 添加表格XML（直接拼接）
            xmlBuilder.AppendLine();
            for (int i = 0; i < xmlForm.Count; i++)
            {
                xmlBuilder.Append(xmlForm);
            }
            xmlBuilder.AppendLine("  </TaxReturn>");
            xmlBuilder.AppendLine("</Payload>");

            return xmlBuilder.ToString();
        }

        private string EscapeXml(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return input
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }
    }
}
