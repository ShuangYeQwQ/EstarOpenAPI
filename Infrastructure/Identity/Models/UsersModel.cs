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
        public string id { get; set; }
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
        public string Username { get; set; }
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
        public DateTime Createdate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime Updatedate { get; set; }
        /// <summary>
        /// token
        /// </summary>
        public string Access_token { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime Expires_in { get; set; }
        /// <summary>
        /// Refieshtoken
        /// </summary>
        public string Refiesh_token { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string Nickname { get; set; }
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

    }

    public class UserServiceModel
    {
        public string Id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public string UId { get; set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 服务年份
        /// </summary>
        public string Year { get; set; }
        /// <summary>
        /// 服务id
        /// </summary>
        public string ServiceId { get; set; }
        /// <summary>
        /// 服务状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 服务开始时间
        /// </summary>
        public DateTime BeginDate { get; set; }
        /// <summary>
        /// 服务结束时间
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 员工id
        /// </summary>
        public string SId { get; set; }
        /// <summary>
        /// 员工id
        /// </summary>
        public string SId1 { get; set; }
        /// <summary>
        /// 员工id
        /// </summary>
        public string SId2 { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Descs { get; set; }
    }
    public class UserOrderModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 下单用户编号
        /// </summary>
        public string Uid { get; set; }
        /// <summary>
        /// 下单用户账户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 购买项目id
        /// </summary>
        public string UserServiceId { get; set; }
        /// <summary>
        /// 订单创建时间
        /// </summary>
        public DateTime Createtime { get; set; }
        /// <summary>
        /// 支付平台
        /// </summary>
        public string PaymentPlatform { get; set; }
        /// <summary>
        /// 购买时间
        /// </summary>
        public DateTime PayTime { get; set; }
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
        public string currencycode { get; set; }
        

    }
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
