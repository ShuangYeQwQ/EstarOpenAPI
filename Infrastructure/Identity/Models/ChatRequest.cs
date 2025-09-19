using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity.Models
{
    public class ChatRequest
    {
        public string Message { get; set; }
        public Guid? FileId { get; set; }
    }
}
