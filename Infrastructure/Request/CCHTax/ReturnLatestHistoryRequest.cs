using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Infrastructure.Request.CCHTax
{
    public class ReturnLatestHistoryRequest
    {
        public List<string> Activities { get; set; } 
        public List<string> ReturnId { get; set; } 
    }
}
