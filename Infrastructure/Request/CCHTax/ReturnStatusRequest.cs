using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Request.CCHTax
{
    public class ReturnStatusRequest
    {
        public List<string> ReturnId { get; set; }
        public string Status { get; set; }
    }
}
