using Application.RequestModel;
using Application.RequestModel.TaskPage;
using Application.ResponseModel.ServicePage;
using Application.ResponseModel.Task;
using Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITaskHandlerSservice
    {
        /// <summary>
        ///添加客户任务
        /// </summary>
        /// <returns></returns>
        Task<Response<int>> AddUserTaskAsync(common_req<Employeetask_res> signup_req);
        /// <summary>
        ///添加客户任务
        /// </summary>
        /// <returns></returns>
        Task<Response<int>> UpdateUserTaskStatusAsync(common_req<UpdataTaskStatus_req> signup_req);
        
    }
}
