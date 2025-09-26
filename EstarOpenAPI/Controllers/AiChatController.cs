using Application.Interfaces;
using Application.RequestModel;
using Application.RequestModel.AccountPage;
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
        /// <param name="signup_req"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument([FromBody] common_req<IFormFile> signup_req)
        {
            if (signup_req.Actioninfo == null || signup_req.Actioninfo.Length == 0) return Ok("文件为空");
            return Ok(await _aichatService.UploadDocument(signup_req));
        }
    }
}
