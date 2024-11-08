using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloudModel
{
    public class Users
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string Uid { get; set; }
        /// <summary>
        /// 权限id
        /// </summary>
        public string Roleid { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Createdate { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime Updatedate { get; set; }
        /// <summary>
        /// 登录token
        /// </summary>
        public string Access_token { get; set; }
        /// <summary>
        /// token刷新时间
        /// </summary>
        public DateTime Expires_in { get; set; }
        /// <summary>
        /// 刷新token
        /// </summary>
        public string Refiesh_token { get; set; }
        /// <summary>
        /// 昵称
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
        /// 状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobilephone { get; set; }
        /// <summary>
        /// 用户公司域
        /// </summary>
        public string Domain { get; set; }
    }
}
