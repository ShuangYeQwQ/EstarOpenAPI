using Application.RequestModel;
using Application.RequestModel.HomePage;
using Application.RequestModel.PayPage;
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
        Task<Response<string>> StripePaySuccessAsync(common_req<PayPayPayment_req> signup_req);
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        Task<Response<int>> CreatePayPalSourceAsync(common_req<PayPal_req> signup_req);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        Task<Response<string>> PaymentSuccess(common_req<PayPayPayment_req> signup_req);
        
    }
}
