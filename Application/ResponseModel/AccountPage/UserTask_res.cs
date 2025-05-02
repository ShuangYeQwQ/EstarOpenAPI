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
    /// <summary>
    /// 任务详情
    /// </summary>
    public class UserTaskDetail_res
    {

        /// <summary>
        /// 需处理的用户服务id
        /// </summary>
        public string userServiceDetailId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string createTime { get; set; }

        /// <summary>
        /// 发送人
        /// </summary>
        public string sendUser { get; set; }

        /// <summary>
        /// 任务标题
        /// </summary>
        public string taskTitle { get; set; }

        /// <summary>
        /// 任务内容
        /// </summary>
        public string taskContent { get; set; }

        /// <summary>
        /// 任务对应服务名称
        /// </summary>
        public string serviceName { get; set; }

        /// <summary>
        /// 服务创建时间
        /// </summary>
        public string begindate { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public string type { get; set; }
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
