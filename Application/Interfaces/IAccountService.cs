using Application.RequestModel;
using Application.RequestModel.AccountPage;
using Application.RequestModel.HomePage;
using Application.ResponseModel.AccountPage;
using Application.ResponseModel.HomePage;
using Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAccountService
    {
        /// <summary>
        ///创建用户基本信息
        /// </summary>
        /// <returns></returns>
        Task<Response<string>> CreateUserInformationAsync(common_req<UserInformation_req> signup_req);
        /// <summary>
        ///获取用户基本信息
        /// </summary>
        /// <returns></returns>
        Task<Response<User_res>> GetUserAsync(common_req<User_req> signup_req);
        
    }
}
