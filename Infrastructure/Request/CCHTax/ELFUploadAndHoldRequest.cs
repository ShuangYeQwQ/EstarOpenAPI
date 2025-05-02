using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Infrastructure.Request.CCHTax
{
    public class ELFUploadAndHoldRequest
    {
        [JsonPropertyName("ReturnID")]
        public string ReturnID { get; set; }

        [JsonPropertyName("ClientGuid")]
        public string ClientGuid { get; set; }

        [JsonPropertyName("ClientIPAddress")]
        public string ClientIPAddress { get; set; }

        [JsonPropertyName("DeviceID")]
        public string DeviceID { get; set; }

        [JsonPropertyName("DiskDriveSerialNumber")]
        public string DiskDriveSerialNumber { get; set; }

        [JsonPropertyName("UnitCodes")]
        public List<string> UnitCodes { get; set; }

        [JsonPropertyName("ExportType")]
        public string ExportType { get; set; }

        [JsonPropertyName("ComplianceMessageResponses")]
        public List<ComplianceMessageResponse> ComplianceMessageResponses { get; set; } = new();
    }

    // ComplianceMessageResponse 类定义
    public class ComplianceMessageResponse
    {
        [JsonPropertyName("MessageID")]
        public string MessageID { get; set; }

        [JsonPropertyName("MessageText")]
        public string MessageText { get; set; }
    }
}
