using Application.RequestModel;
using Application.RequestModel.HomePage;
using Application.ResponseModel.HomePage;
using Application.Wrappers;

namespace Application.Interfaces
{
    public interface IHomePageService
    {
        /// <summary>
        ///注册账号
        /// </summary>
        /// <returns></returns>
        Task<Response<Signup_res>> RegisterAsync(common_req<Signup_req> signup_req);
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="type">注册/登录</param>
        /// <param name="email">收件人email</param>
        /// <returns></returns>
        Task<Response<string>> SendVerificationEmailAsync(common_req<Signup_req> signup_req);
         /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="type">注册/登录</param>
        /// <param name="email">收件人email</param>
        /// <returns></returns>
        Task<Response<Signup_res>> LoginAsync(common_req<Signup_req> signup_req);
    }
}
