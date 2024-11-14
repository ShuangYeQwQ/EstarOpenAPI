using Application.RequestModel.HomePage;
using Application.ResponseModel.HomePage;
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
        Task<Response<string>> CreatesourceAsync();
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        Task<Response<string>> CreatePayPalSourceAsync();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        Task<Response<string>> PaymentSuccess(string token, string PayerID);
        
    }
}
