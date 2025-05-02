using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Response.CCHTax
{
    public class ELFReturnUnit
    {
        public string UnitName { get; set; } = string.Empty;
        public string UnitStatus { get; set; } = string.Empty;
        public string PreviousFillingStatus { get; set; } = string.Empty;
    }
}
