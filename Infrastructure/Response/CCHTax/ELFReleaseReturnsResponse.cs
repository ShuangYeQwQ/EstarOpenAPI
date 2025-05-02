using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Response.CCHTax
{
    public class ELFReleaseReturnsResponse
    {
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
