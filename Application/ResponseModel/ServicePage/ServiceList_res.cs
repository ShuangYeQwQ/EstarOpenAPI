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
        /// <summary>
        /// 服务总数
        /// </summary>
        public int totalCount { get; set; }
        /// <summary>
        /// 服务总页数
        /// </summary>
        public int totalPages { get; set; }
        //服务列表
        public List<UnfinishedServiceList_res> unfinishedServiceList { get; set; }
    }

    //已完成服务列表
    public class UnfinishedServiceList_res
    {
        //服务id
        public string id { get; set; }
        //用户名称
        public string nickName { get; set; }
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

    //用户服务详情
    public class UserServiceDetail_res
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 已支付金额
        /// </summary>
        public string PayAmount { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string Begindate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string Enddate { get; set; }
        /// <summary>
        /// 员工id
        /// </summary>
        public string OrdinaryEmployees { get; set; }
        /// <summary>
        /// 员工id
        /// </summary>
        public string ExpertEmployees { get; set; }
        /// <summary>
        /// 员工id
        /// </summary>
        public string ProfessionalEmployees { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OrdinaryEmployeesName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ExpertEmployeesName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProfessionalEmployeesName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string taskId { get; set; }

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

    public class ServiceDeatil_res
    {
        //服务id
        public string Id { get; set; }
        //服务名
        public string ServiceName { get; set; }
        //
        public string ServiceNameDesc { get; set; }
        //基础价格
        public string Amount { get; set; }
        // 类别1
        public string ServiceLevel1 { get; set; }
        //类别2
        public string ServiceLevel2 { get; set; }
        //类别3
        public string ServiceLevel3 { get; set; }
        //备注
        public string Descs { get; set; }
        //附加服务
        public List<ServiceDeatil_res> AdditionalList { get; set; }
        //包
        public List<ServiceDeatil_res> ServicePackage { get; set; }
        //服务变量
        public List<ServiceItem_res> serviceItems { get; set; } 

    }
    public class ServicePackageDeatil_res
    {
        //服务id
        public string Id { get; set; }
        //服务名
        public string ServiceName { get; set; }
        //
        public string ServiceNameDesc { get; set; }
        //折扣
        public string Discount { get; set; }
        // 类别1
        public string ServiceLevel1 { get; set; }
        //类别2
        public string ServiceLevel2 { get; set; }
        //类别3
        public string ServiceLevel3 { get; set; }
        //备注
        public string Descs { get; set; }
        /// <summary>
        /// 包下面的服务
        /// </summary>
        public List<PackageService> PackageServices { get; set; }

        public class PackageService
        {
            public string ServiceId { get; set; } 
            //服务名
            public string ServiceName { get; set; }
            //
            public string ServiceNameDesc { get; set; }
            //基础价格
            public string Amount { get; set; }
            // 类别1
            public string ServiceLevel1 { get; set; }
            //类别2
            public string ServiceLevel2 { get; set; }
            //类别3
            public string ServiceLevel3 { get; set; }
            //备注
            public string Descs { get; set; }
            //附加服务`
            public List<ServiceDeatil_res> AdditionalList { get; set; }
            //服务变量
            public List<ServiceItem_res> serviceItems { get; set; }
        }
    }
    
}
