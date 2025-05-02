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
    public class ServicesController : Controller
    {

        private readonly IServicesService _servicesService;
        public ServicesController(IServicesService servicesService)
        {
            _servicesService = servicesService;
        }
        /// <summary>
        /// 用户服务显示模板，每个服务未完成个数
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        [HttpPost("getservicecount")]
        public async Task<IActionResult> GetServiceCountAsync([FromBody] common_req<string> signup_req)
        {
            return Ok(await _servicesService.GetServiceCountAsync(signup_req));
        }
        /// <summary>
        /// 获取用户任务信息
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        [HttpPost("getusertasklist")]
        public async Task<IActionResult> GetUserTaskListAsync([FromBody] common_req<string> signup_req)
        {
            return Ok(await _servicesService.GetUserTaskListAsync(signup_req));
        }
        /// <summary>
        /// 获取用户任务详情
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        [HttpPost("getusertaskdetail")]
        public async Task<IActionResult> GetUserTaskDetailAsync([FromBody] common_req<string> signup_req)
        {
            return Ok(await _servicesService.GetUserTaskDetailAsync(signup_req));
        }
        /// <summary>
        /// 修改用户任务状态
        /// </summary>
        /// <param name="signup_req">type:任务状态，actioninfo：任务id</param>
        /// <returns></returns>
        [HttpPost("updateusertaskstatus")]
        public async Task<IActionResult> UpdateUserTaskStatusAsync([FromBody] common_req<string> signup_req)
        {
            return Ok(await _servicesService.UpdateUserTaskStatusAsync(signup_req));
        }
        /// <summary>
        /// 服务列表
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        [HttpPost("getservicelist")]
        public async Task<IActionResult> GetServiceListAsync([FromBody] common_req<UserService_req> signup_req)
        {
            return Ok(await _servicesService.GetServiceListAsync(signup_req));
        }
        /// <summary>
        /// 服务下变量列表
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        [HttpPost("getserviceitemlist")]
        public async Task<IActionResult> GetServiceItemListAsync([FromBody] common_req<string> signup_req)
        {
            return Ok(await _servicesService.GetServiceItemListAsync(signup_req));
        }
        /// <summary>
        /// 用户上传文件，识别文件表名
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        [HttpPost("getgoogledocumentaiformname")]
        public async Task<IActionResult> GoogleDocumentAIGetFormNameAsync([FromBody] common_req<string> signup_req)
        {
            return Ok(await _servicesService.GoogleDocumentAIGetFormNameAsync(signup_req));
        }
        /// <summary>
        /// 获取服务信息
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        [HttpPost("getservicedetail")]
        public async Task<IActionResult> GetServiceDetail([FromBody] common_req<string> signup_req)
        {
            return Ok(await _servicesService.GetServiceDetail(signup_req));
        }
        /// <summary>
        /// 获取服务包信息
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        [HttpPost("getservicepackagedetail")]
        public async Task<IActionResult> GetServicePackageDetail([FromBody] common_req<string> signup_req)
        {
            return Ok(await _servicesService.GetServicePackageDetail(signup_req));
        }
    }
}
