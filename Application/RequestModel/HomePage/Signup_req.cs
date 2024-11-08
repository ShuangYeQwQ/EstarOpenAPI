namespace EstarOpenAPI.Application.RequestModel.HomePage
{

    public class Signup_req
    {

        /// <summary>
        /// 邮箱
        /// </summary>

        public string email { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>

        public string mobilephone { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>

        public string nickname { get; set; }
        /// <summary>
        /// 密码
        /// </summary>

        public string password { get; set; }
        /// <summary>
        /// 邮箱验证码
        /// </summary>

        public string verificationcode { get; set; }

    }
}
