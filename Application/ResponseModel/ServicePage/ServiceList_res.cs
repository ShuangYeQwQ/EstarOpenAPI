using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseModel.ServicePage
{
    //服务列表
    public class ServiceList_res
    {
        //个人会计
        public List<ServiceListCount_res> personalAccounting { get; set; }
        //个人保险
        public List<ServiceListCount_res> personalInsurance { get; set; }
        //公司会计
        public List<ServiceListCount_res> companyAccounting { get; set; }
        //公司保险
        public List<ServiceListCount_res> companyInsurance { get; set; }
    }
    //用户服务列表
    public class UserServiceList_res
    {
        //已完成服务列表
        public List<CompleteServiceList_res> completeServiceList { get; set; }
        //未完成服务列表
        public List<UnfinishedServiceList_res> unfinishedServiceList { get; set; }
    }

    //已完成服务列表
    public class CompleteServiceList_res
    {
        //服务id
        public string id { get; set; }
        //用户名称
        public string userName { get; set; }
        //服务名称
        public string serviceName { get; set; }
        //服务使用年份
        public string serviceDate { get; set; }
        //服务状态
        public string serviceStatus { get; set; }
        //开始时间
        public string beginDate { get; set; }
        //结束时间
        public string endDate { get; set; }
        //服务状态
        public string status { get; set; }
    }
    //未完成服务列表
    public class UnfinishedServiceList_res
    {
        //服务id
        public string id { get; set; }
        //用户名称
        public string userName { get; set; }
        //服务名称
        public string serviceName { get; set; }
        //服务使用年份
        public string serviceDate { get; set; }
        //服务状态 0：已下单,（待上传文件），1：已处理待员工1审核/ 2：员工1已审核待员工2审核/ 3：员工2已审核待员工3审核/ 4：员工3已审核待员工2发送税表，签字，账单文件/ 5：已发送文件待客户处理/ 10：已完成
        public string serviceStatus { get; set; }
        //开始时间
        public string beginDate { get; set; }

    }
    //首页服务未完成个数
    public class ServiceListCount_res
    {
        //服务id
        public string id { get; set; }
        //服务名称
        public string name { get; set; }
        //用户购买未完成的服务数
        public int serviceCount { get; set; }
        
    }

}
