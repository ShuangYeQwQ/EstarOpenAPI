using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseModel.AccountPage
{
    public class User_res
    {
        //用户id
        public string Id { get; set; }
        //用户昵称
        public string Nickname { get; set; }
        //用户性别
        public string Gender { get; set; }
        //用户头像
        public string Avatar { get; set; }
        //用户生日
        public string Birthdate { get; set; }
        //用户手机
        public string Mobilephone { get; set; }
        //国家代码
        public string CountryCode { get; set; }
        //国家/地区中最高级别的细分，通常是省、州或 ISO-3166-2 细分
        public string AdminArea1 { get; set; }
        //城市、城镇或村庄
        public string AdminArea2 { get; set; }
        //地址的第一行，例如门牌号和街道
        public string AddressLine1 { get; set; }
        //地址的第二行，例如套房或公寓号
        public string AddressLine2 { get; set; }
        //googleid
        public string GooglelocalId { get; set; }
        //是否启用google mfa邮箱验证 
        public string EmailVerification { get; set; }
        //是否启用google mfas手机验证 
        public string PhoneVerification { get; set; }
        //登录账户身份 
        public string UserRole { get; set; }
        //显示页面内容列表 
        public string UserPageList { get; set; }
    }

}
