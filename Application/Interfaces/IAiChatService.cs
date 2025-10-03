using Application.RequestModel;
using Application.RequestModel.Chat;
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
        Task<Response<List<string>>> UploadDocument(List<IFormFile> files, string userId, string type);
        /// <summary>
        ///根据用户文本获取关联回答
        /// </summary>
        /// <returns></returns>
        Task<Response<string>> UserTextChat(common_req<AiChat_req> signup_req);
    }
}
