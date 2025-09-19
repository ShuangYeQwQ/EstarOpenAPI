using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.RequestModel.TaskPage
{
    public class Task_req
    {
        
    }
    public class UpdataTaskStatus_req
    {
        //任务id
        public string id {  get; set; } 
        //任务状态
        public string status { get; set; }
        //内容
        public string content { get; set; }
    }
}
