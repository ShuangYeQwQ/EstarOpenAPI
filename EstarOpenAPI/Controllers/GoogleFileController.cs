using Application.Interfaces;
using Application.RequestModel;
using Application.RequestModel.AccountPage;
using Application.RequestModel.GoogleFile;
using Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Mvc;

namespace EstarOpenAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class GoogleFileController : Controller
    {
        private readonly IGoogleFileService _googleFileService;
        public GoogleFileController(IGoogleFileService googleFileService)
        {
            _googleFileService = googleFileService;
        }

        /// <summary>
        /// 获取上传文件至Google Cloud Storage的签名URL       
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        [HttpPost("generatesigneduploadurl")]
        public async Task<IActionResult> GenerateSignedUploadUrl(common_req<UploadGoogleFile_req> signup_req)
        {
            return Ok(_googleFileService.GenerateSignedUploadUrl(signup_req));
        }
    }
}
