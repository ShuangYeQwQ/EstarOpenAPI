using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseModel.Task
{
    public class UpdateTaskStatus_res
    {
        /// <summary>
        /// 任务id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 用户服务状态
        /// </summary>
        public string UserServiceStatus { get; set; }
    }
    //员工添加任务
    public class Employeetask_res
    {
        /// <summary>
        /// 需处理的用户服务id
        /// </summary>
        public string UserServiceDetailId { get; set; }
        /// <summary>
        /// 发送人id
        /// </summary>
        public string Sid { get; set; }
        /// <summary>
        /// 任务标题
        /// </summary>
        public string TaskTitle { get; set; }
        /// <summary>
        /// 任务内容
        /// </summary>
        public string TaskContent { get; set; }
        /// <summary>
        /// 类型：0：需上传文件，1：发送文本说明
        /// </summary>
        public string Type { get; set; }

    }

    //列表
    public class TaskList_res
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int Uid { get; set; }
        /// <summary>
        /// 需处理的用户服务id
        /// </summary>
        public int UserServiceDetailId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreateTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public int UpdateTime { get; set; }
        /// <summary>
        /// 发送人id
        /// </summary>
        public int Sid { get; set; }
        /// <summary>
        /// 任务标题
        /// </summary>
        public int TaskTitle { get; set; }
        /// <summary>
        /// 任务内容
        /// </summary>
        public int TaskContent { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 类型：0：需上传文件，1：发送文本说明
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 0:系统发送,1：员工发送
        /// </summary>
        public int SendType { get; set; }

    }


}
