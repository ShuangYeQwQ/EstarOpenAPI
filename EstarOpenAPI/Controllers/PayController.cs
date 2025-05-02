using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.RequestModel.HomePage;
using Application.RequestModel;
using static Google.Cloud.DocumentAI.V1.Document.Types.Provenance.Types;
using System;
using Infrastructure.Identity.Services;
using Application.RequestModel.PayPage;

namespace EstarOpenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayController : ControllerBase
    {
        private readonly ILogger<PayController> _logger;
        private readonly IPayService _payService;
        public PayController(ILogger<PayController> logger, IPayService payService)
        {
            _logger = logger;
            _payService = payService;
        }
        /// <summary>
        /// 创建stripe支付窗口
        /// </summary>
        /// <param name="signup_req">付款类型:个人报税付款,公司报税付款,保险付款</param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost("createstripepay")]
        public async Task<IActionResult> CreateStripePayAsync([FromBody] common_req<string> signup_req)
        {
            return Ok(await _payService.CreateStripePayAsync());
        }
        /// <summary>
        /// stripe支付成功
        /// </summary>
        /// <param name="signup_req">付款类型:个人报税付款,公司报税付款,保险付款</param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost("stripepaysuccess")]
        public async Task<IActionResult> StripePaySuccessAsyncAsync([FromBody] common_req<PayPayPayment_req> signup_req)
        {
            return Ok(await _payService.StripePaySuccessAsync(signup_req));
        }

        /// <summary>
        /// 创建paypal支付窗口
        /// </summary>
        /// <param name="signup_req">付款类型:个人报税付款,公司报税付款,保险付款</param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost("createpaypalsource")]
        public async Task<IActionResult> CreatePayPalSourceAsync([FromBody] common_req<PayPal_req> signup_req)
        {
            // https://localhost:44386/api/Pay/createpaypalsource
            return Ok(await _payService.CreatePayPalSourceAsync(signup_req));
        }
        /// <summary>
        /// paypal支付
        /// </summary>
        /// <param name="token">订单id</param>string token, string PayerID
        /// <param name="PayerID"></param>
        /// <returns></returns>
        [HttpPost("payment-success")]
        public async Task<IActionResult> PaymentSuccessAsync([FromBody] common_req<PayPayPayment_req> signup_req)
        {
            return Ok(await _payService.PaymentSuccess(signup_req));
        }
       
    }
}
