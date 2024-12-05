using Application.Interfaces;
using Application.RequestModel;
using Application.RequestModel.AccountPage;
using Microsoft.AspNetCore.Mvc;

namespace EstarOpenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        /// <summary>
        /// 创建用户基本信息
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        [HttpPost("createuserinformation")]
        public async Task<IActionResult> CreateUserInformationAsync([FromBody] common_req<UserInformation_req> signup_req)
        {
            return Ok(await _accountService.CreateUserInformationAsync(signup_req));
        }
        /// <summary>
        /// 获取用户基本信息
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        [HttpPost("getuser")]
        public async Task<IActionResult> GetUserAsync([FromBody] common_req<User_req> signup_req)
        {
            return Ok(await _accountService.GetUserAsync(signup_req));
        }
        
    }
}
