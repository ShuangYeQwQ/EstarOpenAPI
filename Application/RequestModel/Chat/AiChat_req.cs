using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.RequestModel.Chat
{
    public class AiChat_req
    {
        public string CustomerId { get; set; }
        public string Question { get; set; }
        public int TopK { get; set; } = 5;
    }

}
