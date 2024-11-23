using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Application.RequestModel.GoogleIdentityPlatform
{
    public class GooglesignUp_req
    {
        public string email { get; set; }
        public string PhoneNumber { get; set; }
        public string password { get; set; }
        public bool returnSecureToken { get; set; }
        public List<MfaFactor> mfaInfo { get; set; }
        public class MfaFactor
        {
            public string phoneInfo { get; set; }
        }
    }
}
