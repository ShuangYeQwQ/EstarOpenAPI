using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Response.CCHTax
{
    public class ReturnInfo
    {
        public string ReturnId { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string OfficeName { get; set; } = string.Empty;
    }

    public class AdditionalInfo
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class BatchItem
    {
        public string ItemGuid { get; set; } = string.Empty;
        public string ItemStatusCode { get; set; } = string.Empty;
        public string ItemStatusDescription { get; set; } = string.Empty;
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseDescription { get; set; } = string.Empty;
        public ReturnInfo ReturnInfo { get; set; } = new ReturnInfo();
        public List<AdditionalInfo> AdditionalInfo { get; set; } = new List<AdditionalInfo>();
    }

    public class BatchStatusResponse
    {
        public string BatchStatus { get; set; } = string.Empty;
        public string BatchStatusDescription { get; set; } = string.Empty;
        public List<BatchItem> Items { get; set; } = new List<BatchItem>();
    }
}
