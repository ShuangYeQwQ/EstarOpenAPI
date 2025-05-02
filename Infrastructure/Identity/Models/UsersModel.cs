using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity.Models
{

    public class UsersModel
    {
        
        /// <summary>
        /// 用户id
        /// </summary>
        public string Uid { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户手机号
        /// </summary>
        public string Mobilephone { get; set; }
        /// <summary>
        /// 用户email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// token
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpiresIn { get; set; }
        /// <summary>
        /// Refieshtoken
        /// </summary>
        public string RefieshToken { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int Gender { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime Birthdate { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 账号状态，0:未启用，1:已启用,2：失效
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 用户Domain
        /// </summary>
        public string Domain { get; set; }
        /// <summary>
        /// 用户社安号码
        /// </summary>
        public string SocialSecurityNumber { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        public string CountryCode { get; set; }
        /// <summary>
        /// 省、州
        /// </summary>
        public string AdminArea1 { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string AdminArea2 { get; set; }
        /// <summary>
        /// 门牌号和街道
        /// </summary>
        public string AddressLine1 { get; set; }
        /// <summary>
        /// 套房或公寓号
        /// </summary>
        public string Addressline2 { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string PostalCode { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public string Balance { get; set; }
        /// <summary>
        /// Google账户id
        /// </summary>
        public string GooglelocalId { get; set; }
        /// <summary>
        /// 是否验证google邮箱   0:未验证,1：已验证
        /// </summary>
        public string EmailVerification { get; set; }
        /// <summary>
        /// 验证google手机   0:未验证,1：已验证
        /// </summary>
        public string PhoneVerification { get; set; }
        /// <summary>
        /// 是否购买过服务  0:未购买，1：购买过
        /// </summary>
        public string FirstBuy { get; set; }

    }

    //用户服务
    public class UserServiceModel
    {
        public Guid Id { get; private set; } = Guid.NewGuid(); // 直接初始化
        /// <summary>
        /// 用户id
        /// </summary>
        public string UId { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 服务id
        /// </summary>
        public string ServiceId { get; set; }
        /// <summary>
        /// 服务状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 首次支付金额
        /// </summary>
        public decimal FirstPayAmount { get; set; }
        /// <summary>
        /// 已支付金额
        /// </summary>
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Descs { get; set; }
    }
    
    //用户服务明细
    public class UserServiceDetailModel {
        public Guid Id { get; private set; } = Guid.NewGuid(); // 直接初始化
        /// <summary>
        /// 服务ID
        /// </summary>
        public string ServiceId { get; set; }
        /// <summary>
        /// 服务主id
        /// </summary>
        public Guid UserServiceId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 服务使用开始时间  报税年份  持续性服务使用开始时间
        /// </summary>
        public DateTime BeginServiceDate { get; set; }
        /// <summary>
        /// 服务使用结束时间 持续性服务使用结束时间
        /// </summary>
        public DateTime EndServiceDate { get; set; }
        /// <summary>
        /// 服务金额
        /// </summary>
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// 已支付金额
        /// </summary>
        public decimal PayAmount { get; set; }

        /// <summary>
        /// 服务开始时间
        /// </summary>
        public DateTime BeginDate { get; set; }
        /// <summary>
        /// 服务结束时间
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 服务计数(持续性)
        /// </summary>
        public int ServiceNumber { get; set; }
        /// <summary>
        /// 持续服务是否结束
        /// </summary>
        public string IsEnd { get; set; }
        /// <summary>
        /// 服务状态
        /// </summary>
        public string Status { get; set; }
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
        /// 员工id
        /// </summary>
        public string AccountingEmployees { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Descs { get; set; }
    
    }

    //用户服务变量
    public class UserServiceItems
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 用户服务详情id
        /// </summary>
        public Guid UserServiceDeatilId { get; set; }
        /// <summary>
        /// 服务计数(持续性)
        /// </summary>
        public string ServiceNumber { get; set; }
        /// <summary>
        /// 服务变量id
        /// </summary>
        public string ServiceItemId { get; set; }
        /// <summary>
        /// 客户输入变量值
        /// </summary>
        public string UserServiceItemValue { get; set; }
        /// <summary>
        /// 客户变量金额
        /// </summary>
        public string UserServiceItemAmount { get; set; }
        /// <summary>
        /// 员工处理后变量值
        /// </summary>
        public string  StaffServiceItemValue{ get; set; }
        /// <summary>
        /// 员工变量金额
        /// </summary>
        public string StaffServiceItemAmount { get; set; }
    }
    
    //用户订单
    public class UserOrderModel
    {
        public Guid Id { get; private set; } = Guid.NewGuid(); // 直接初始化
        /// <summary>
        /// 下单用户编号
        /// </summary>
        public string Uid { get; set; }
        /// <summary>
        /// 订单创建时间
        /// </summary>
        public DateTime Createtime { get; set; }
        /// <summary>
        /// 订单创建时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 付款平台id
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 购买项目id
        /// </summary>
        public Guid UserServiceId { get; set; }
       
        /// <summary>
        /// 购买时间
        /// </summary>
        public DateTime OrderTime { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal PayAmount { get; set; }
        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal DiscountAmount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string OrderNote { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string OrderSource { get; set; }
        /// <summary>
        /// 支付的货币类型
        /// </summary>
        public string CurrencyCode { get; set; }
        
    }

    //用户订单详情
    public class UserOrderDetailModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid OId { get; set; }
        /// <summary>
        /// 订单创建时间
        /// </summary>
        public DateTime Createtime { get; set; }
        /// <summary>
        /// 购买项目id
        /// </summary>
        public string UserServiceDetailId { get; set; }
        /// <summary>
        /// 购买时间
        /// </summary>
        public DateTime PayTime { get; set; }
       
        /// <summary>
        /// 支付平台
        /// </summary>
        public string PaymentPlatform { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string OrderNote { get; set; }
        /// <summary>
        /// 平台订单号
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 金额类型
        /// </summary>
        public string CurrencyCode { get; set; }

    }


    //用户任务
    public class UserTaskModel()
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string Uid { get; set; }
        /// <summary>
        /// 需处理的服务id
        /// </summary>
        public string UserServiceId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 发送人
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
        /// 状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 发送类型0:系统发送，1：员工发送
        /// </summary>
        public int SendType { get; set; }
    }
}
