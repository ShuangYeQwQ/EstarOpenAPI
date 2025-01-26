using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseModel.AccountPage
{
    public class UserTask_res
    {
        /// <summary>
        /// 未完成任务列表
        /// </summary>
        public List<UserUnfinishedTask> userUnfinishedTasks { get; set; }
        /// <summary>
        /// 已完成项目列表
        /// </summary>
        public List<UserCompleteServices> userCompleteServices { get; set; }
    }
    public class UserTaskDetail_res
    {
        
    }
    public class UserUnfinishedTask
    {
        /// <summary>
        /// 任务id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 任务标题
        /// </summary>
        public string taskTitle { get; set; }
        /// <summary>
        /// 任务内容
        /// </summary>
        public string taskContent { get; set; }
        /// <summary>
        /// 任务类型 上传文件/确认服务
        /// </summary>
        public string taskType { get; set; }
    }
    public class UserCompleteServices
    {
        
    }
}
