using Application.RequestModel;
using Application.Wrappers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public  interface IAiChatService
    {
        /// <summary>
        ///客户上传个人文件
        /// </summary>
        /// <returns></returns>
        Task<Response<string>> UploadDocument(common_req<IFormFile> signup_req);
    }
}
