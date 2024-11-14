using Application.Interfaces;
using Application.ResponseModel.HomePage;
using Application.Wrappers;
using Microsoft.Extensions.Configuration;
using PaypalServerSdk.Standard.Authentication;
using PaypalServerSdk.Standard.Exceptions;
using PaypalServerSdk.Standard.Http.Response;
using PaypalServerSdk.Standard.Models;
using PaypalServerSdk.Standard;
using Stripe;
using Stripe.Checkout;
using Stripe.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Identity.Services
{
    public class PayService : IPayService
    {
        public ILogger<HomePageService> _logger { get; }
        public PayService()
        {
        }
        /// <summary>
        /// 创建stripe支付窗口
        /// </summary>
        /// <returns>stripe支付ClientSecret</returns>
        public async Task<Response<string>> CreatesourceAsync()
        {
            Signup_res signup_Res = new Signup_res();
            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = 10000,  // Amount in cents (e.g., $20.00 = 2000)
                    Currency = "usd",
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                    },
                    Metadata = new Dictionary<string, string> { { "order_id", "6735" } },
                };


                var service = new PaymentIntentService();
                var session = service.Create(options);
                return new Response<string>(session.ClientSecret, "");
            }
            catch (StripeException ex)
            {
                return new Response<string>(ex.Message);
            }
        }
        public async Task<Response<string>> CreatePayPalSourceAsync()
        {
            PaypalServerSdkClient client = new PaypalServerSdkClient.Builder()
   .ClientCredentialsAuth(
       new ClientCredentialsAuthModel.Builder(
           "Ab6ox0iQPfv4YavdSohekbvHLnh6p_3eFZSHLTwMz-yjoPvoDZ0FRPqPjtvUuZ6vwyANuKKuQJD6QPF-",
           "EOSYI4iyvI09Fofy-P39aVMpnylGUQICRRNvtpDaj_xi3R5HkTfz9RHYTQiTMvf5u8HkoKqdhcXTbhkj"
       )
       .Build())
   .Environment(PaypalServerSdk.Standard.Environment.Sandbox)
   .LoggingConfig(config => config
       .LogLevel(LogLevel.Information)
       .RequestConfig(reqConfig => reqConfig.Body(true))
       .ResponseConfig(respConfig => respConfig.Headers(true))
            )
   .Build();
            var ordersController = client.OrdersController;
            OrdersCreateInput ordersCreateInput = new OrdersCreateInput
            {
                Body = new OrderRequest
                {
                    Intent = CheckoutPaymentIntent.Authorize,
                    PurchaseUnits = new List<PurchaseUnitRequest>

        {
            new PurchaseUnitRequest
            {
                Amount = new AmountWithBreakdown
                {
                    CurrencyCode = "usd",
                    MValue = "100",
                },
                Description = "该订单仅包含基础服务，不包含（收入抵扣,所得税减免,已付税款）服务"
            },
        },
                    PaymentSource = new PaymentSource
                    {
                        Paypal = new PaypalWallet
                        {
                            ExperienceContext = new PaypalWalletExperienceContext
                            {
                                ReturnUrl = "https://localhost:44386/api/Pay/payment-success",
                                CancelUrl = "https://localhost:44386/api/Pay/payment-cancel",
                                UserAction = PaypalExperienceUserAction.PayNow
                            }
                        }
                    }
                },
                Prefer = "return=minimal",
            };

            try
            {
                ApiResponse<Order> orderresult = await ordersController.OrdersCreateAsync(ordersCreateInput);
                string approveUrl = orderresult.Data.Links.First(link => link.Rel == "payer-action").Href;
                return new Response<string>(approveUrl, "");
                //OrdersAuthorizeInput ordersAuthorizeInput = new OrdersAuthorizeInput
                // {
                //   Id = orderresult.Data.Id,
                //    Prefer = "return=minimal",
                //  };
                //ApiResponse<OrderAuthorizeResponse> authorizeresult = await ordersController.OrdersAuthorizeAsync(ordersAuthorizeInput);
                //string authorizeid = authorizeresult.Data.Intent;
            }
            catch (ApiException e)
            {
                // TODO: Handle exception here
                return new Response<string>(e.Message);
            }
        }

        public async Task<Response<string>> PaymentSuccess(string token, string PayerID)
        {
            PaypalServerSdkClient client = new PaypalServerSdkClient.Builder()
    .ClientCredentialsAuth(
        new ClientCredentialsAuthModel.Builder(
            "Ab6ox0iQPfv4YavdSohekbvHLnh6p_3eFZSHLTwMz-yjoPvoDZ0FRPqPjtvUuZ6vwyANuKKuQJD6QPF-",
            "EOSYI4iyvI09Fofy-P39aVMpnylGUQICRRNvtpDaj_xi3R5HkTfz9RHYTQiTMvf5u8HkoKqdhcXTbhkj"
        )
        .Build())
    .Environment(PaypalServerSdk.Standard.Environment.Sandbox)
    .LoggingConfig(config => config
        .LogLevel(LogLevel.Information)
        .RequestConfig(reqConfig => reqConfig.Body(true))
        .ResponseConfig(respConfig => respConfig.Headers(true))
             )
    .Build();
            OrdersGetInput ordersGetInput = new OrdersGetInput
            {
                Id = token,
            };

            try
            {
                var ordersController = client.OrdersController;
                var paymentsController = client.PaymentsController;
                ApiResponse<Order> result = await ordersController.OrdersGetAsync(ordersGetInput);
                OrdersAuthorizeInput ordersAuthorizeInput = new OrdersAuthorizeInput
                {
                    Id = token,
                    Prefer = "return=minimal",
                };
                //授权付款
                ApiResponse<OrderAuthorizeResponse> ordersresult = await ordersController.OrdersAuthorizeAsync(ordersAuthorizeInput);
                string Authorizationsid = ordersresult.Data.PurchaseUnits[0].Payments.Authorizations[0].Id;
                //
                //ApiResponse<PaymentAuthorization> authorizationsresult = await paymentsController.AuthorizationsGetAsync(Authorizationsid);
                AuthorizationsCaptureInput authorizationsCaptureInput = new AuthorizationsCaptureInput
                {
                    AuthorizationId = Authorizationsid,
                    Prefer = "return=minimal",
                    Body = new CaptureRequest
                    {
                        FinalCapture = false,
                    },
                };
                //付款
                ApiResponse<CapturedPayment> paymentsresult = await paymentsController.AuthorizationsCaptureAsync(authorizationsCaptureInput);
                string paystatus = paymentsresult.Data.Status + "";
                return new Response<string>("", "");
            }
            catch (ApiException e)
            {
                // TODO: Handle exception here
                return new Response<string>(e.Message);
            }
        }
    }
}
