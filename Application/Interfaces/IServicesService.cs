using Application.RequestModel;
using Application.ResponseModel.AccountPage;
using Application.ResponseModel.GoogleDocumentAI;
using Application.ResponseModel.ServicePage;
using Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IServicesService
    {
        /// <summary>
        ///获取可购买服务
        /// </summary>
        /// <returns></returns>
        Task<Response<ServiceList_res>> GetServiceCountAsync(common_req<string> signup_req);
        /// <summary>
        ///获取用户任务信息
        /// </summary>
        /// <returns></returns>
        Task<Response<UserTask_res>> GetUserTaskListAsync(common_req<string> signup_req);
        /// <summary>
        ///获取用户任务详情
        /// </summary>
        /// <returns></returns>
        Task<Response<UserTaskDetail_res>> GetUserTaskDetailAsync(common_req<string> signup_req);
        /// <summary>
        ///修改用户任务状态
        /// </summary>
        /// <returns></returns>
        Task<Response<string>> UpdateUserTaskStatusAsync(common_req<string> signup_req);
        /// <summary>
        ///用户已完成，未完成服务列表
        /// </summary>
        /// <returns></returns>
        Task<Response<UserServiceList_res>> GetServiceListAsync(common_req<string> signup_req);
        /// <summary>
        ///服务下变量列表
        /// </summary>
        /// <returns></returns>
        Task<Response<List<ServiceItem_res>>> GetServiceItemListAsync(common_req<string> signup_req);
        /// <summary>
        ///用户上传文件，识别文件表名
        /// </summary>
        /// <returns></returns>
        Task<Response<List<GoogleDocumentAIFormName_res>>> GoogleDocumentAIGetFormNameAsync(common_req<string> signup_req);

    }
}
