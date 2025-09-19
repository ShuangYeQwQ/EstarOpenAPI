using Application.RequestModel;
using Application.RequestModel.GoogleFile;
using Application.ResponseModel.PayPage;
using Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IGoogleFileService
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        Response<string> GenerateSignedUploadUrl(common_req<UploadGoogleFile_req> signup_req);
    }
}
