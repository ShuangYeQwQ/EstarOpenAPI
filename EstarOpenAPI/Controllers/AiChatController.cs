using Application.Interfaces;
using Application.RequestModel;
using Application.RequestModel.AccountPage;
using Application.RequestModel.Chat;
using Application.RequestModel.ServicePage;
using Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Mvc;

namespace EstarOpenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AiChatController : Controller
    {

        private readonly IAiChatService _aichatService;
        public AiChatController(IAiChatService aichatService)
        {
            _aichatService = aichatService;
        }

        /// <summary>
        /// 客户上传个人文件
        /// </summary>
        /// <param name="files">文件</param>
        /// <param name="userId">客户id</param>
        /// <param name="type">上传类型 0：保存，1：提问</param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument([FromForm] List<IFormFile> files, [FromForm] string userId, [FromForm] string type)//[FromBody] common_req<IFormFile> signup_req
        {
            if (files == null || files.Count == 0) return Ok("文件为空");
            return Ok(await _aichatService.UploadDocument(files, userId,type));
        }
        /// <summary>
        /// 文本ai识别
        /// </summary>
        /// <param name="files">文件</param>
        /// <param name="userId">客户id</param>
        /// <param name="type">上传类型 0：保存，1：提问</param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<IActionResult> AIRecognizeText([FromBody] common_req<AiChat_req> signup_req)//[FromBody] common_req<IFormFile> signup_req
        {
            return Ok(await _aichatService.UserTextChat(signup_req));
        }
    }
}
