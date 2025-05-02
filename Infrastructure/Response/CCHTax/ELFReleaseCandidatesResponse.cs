using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Response.CCHTax
{
    public class ELFReleaseCandidate
    {
        public string ReturnUnitKeyGuid { get; set; } = string.Empty;
        public List<string> Units { get; set; } = new List<string>();
        public string UploadHistoryGuid { get; set; } = string.Empty;
        public int UploadHistoryKey { get; set; }
    }

    public class ELFReleaseCandidatesResponse
    {
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public List<ELFReleaseCandidate> ReleaseCandidates { get; set; } = new List<ELFReleaseCandidate>();
    }
}
