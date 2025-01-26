using Application.RequestModel;
using Application.RequestModel.HomePage;
using Application.ResponseModel.HomePage;
using Application.ResponseModel.PayPage;
using Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPayService
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        Task<Response<Pay_res>> CreateStripePayAsync();
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        Task<Response<string>> StripePaySuccessAsync(string uid, string PaymentIntentId);
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        Task<Response<string>> CreatePayPalSourceAsync(common_req<string> signup_req);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        Task<Response<string>> PaymentSuccess(string uid,string token, string PayerID);
        
    }
}
