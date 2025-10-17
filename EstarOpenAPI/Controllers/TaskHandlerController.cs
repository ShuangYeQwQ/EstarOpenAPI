using Application.Interfaces;
using Application.RequestModel;
using Application.RequestModel.TaskPage;
using Application.ResponseModel.Task;
using Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Mvc;

namespace EstarOpenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskHandlerController : Controller
    {
        private readonly ITaskHandlerSservice _taskHandlerSservice;
        public TaskHandlerController(ITaskHandlerSservice taskHandlerSservice)
        {
            _taskHandlerSservice = taskHandlerSservice;
        }
        /// <summary>
        /// 添加客户任务
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        [HttpPost("addusertask")]
        public async Task<IActionResult> AddUserTaskAsync([FromBody] common_req<Employeetask_res> signup_req)
        {
            return Ok(await _taskHandlerSservice.AddUserTaskAsync(signup_req));
        }
        /// <summary>
        /// 获取客户上传的表格
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        [HttpPost("updusertaskstaus")]
        public async Task<IActionResult> UpdateUserTaskStatusAsync([FromBody] common_req<UpdataTaskStatus_req> signup_req)
        {
            return Ok(await _taskHandlerSservice.UpdateUserTaskStatusAsync(signup_req));
        } /// <summary>
          /// 修改客户任务状态
          /// </summary>
          /// <param name="signup_req"></param>
          /// <returns></returns>
        [HttpPost("getuserformlist")]
        public async Task<IActionResult> GetUserFormListAsync([FromBody] common_req<string> signup_req)
        {
            return Ok(await _taskHandlerSservice.GetUserFormListAsync(signup_req));
        }

        ///// <summary>
        ///// 处理任务
        ///// </summary>
        ///// <param name="payload"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public IActionResult HandleTask([FromBody] string payload)
        //{
        //    // 处理任务
        //    return Ok(new { status = "Task processed successfully" });
        //}
    }
}
