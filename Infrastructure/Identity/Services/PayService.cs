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
using Infrastructure.Identity.Models;
using Infrastructure.Shared;
using Microsoft.Data.SqlClient;
using System.Data;
using EStarGoogleCloud;
using Google.Rpc;
using Stripe.FinancialConnections;
using Application.ResponseModel.PayPage;
using System.Diagnostics;
using Google;

namespace Infrastructure.Identity.Services
{
    public class PayService : IPayService
    {
        public PayService()
        {
        }
        /// <summary>
        /// 创建stripe支付窗口
        /// </summary>
        /// <returns>stripe支付ClientSecret</returns>
        public async Task<Response<Pay_res>> CreateStripePayAsync()
        {
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
                    Metadata = new Dictionary<string, string> { { "ReferenceId", "AC5EA2C9-7E66-4E37-8340-4273AE354A92" } },
                };


                var service = new PaymentIntentService();
                var session = service.Create(options);
                Pay_res pay_Res = new Pay_res
                {
                    clientsecret = session.ClientSecret,
                    paymentintentid = session.Id
                };
                return new Response<Pay_res>(pay_Res, "");
            }
            catch (StripeException ex)
            {
                Pub.SaveLog(nameof(PayService), $"创建stripe支付窗口失败，错误信息：{ex.Message}");
                return new Response<Pay_res>(ex.Message);
            }
        }

        /// <summary>
        /// stripe支付成功
        /// </summary>
        /// <returns></returns>
        public async Task<Response<string>> StripePaySuccessAsync(string PaymentIntentId)
        {
            if (string.IsNullOrEmpty(PaymentIntentId))
            {
                return new Response<string>("PaymentIntentId is required");
            }

            try
            {
                // 使用 Stripe SDK 查询 PaymentIntent
                var service = new PaymentIntentService();
                var paymentIntent = service.Get(PaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    // 支付成功，记录支付信息
                    var chargeService = new ChargeService();
                    var charge = chargeService.Get(paymentIntent.LatestChargeId);
                    var billingDetails = charge.BillingDetails;
                    AddAccountInformation(billingDetails.Email,"Stripe", PaymentIntentId, charge.Metadata["ReferenceId"], (charge.Amount / 100).ToString(), charge.Currency,billingDetails.Name);
                    return new Response<string>("Payment verified","");
                    
                }

                Pub.SaveLog(nameof(PayService), $"stripe支付创建用户账户失败，错误信息：用户支付状态{paymentIntent.Status}");
                return new Response<string>("Payment not verified");
            }
            catch (Exception ex)
            {
                Pub.SaveLog(nameof(PayService), $"stripe支付创建用户账户失败，错误信息：{ex.Message}");
                return new Response<string>(ex.Message);
            }
        }
        /// <summary>
        /// 创建paypal支付链接
        /// </summary>
        /// <returns></returns>
        public async Task<Response<string>> CreatePayPalSourceAsync()
        {
            string oAuthClientId =  System.Configuration.ConfigurationManager.AppSettings["oAuthClientId"] + "";
            string oAuthClientSecret = System.Configuration.ConfigurationManager.AppSettings["oAuthClientSecret"] + "";
            PaypalServerSdkClient client = new PaypalServerSdkClient.Builder()
   .ClientCredentialsAuth(new ClientCredentialsAuthModel.Builder(oAuthClientId,oAuthClientSecret).Build())
   .Environment(PaypalServerSdk.Standard.Environment.Sandbox)
   .LoggingConfig(config => config
       .LogLevel(LogLevel.Information)
       .RequestConfig(reqConfig => reqConfig.Body(true))
       .ResponseConfig(respConfig => respConfig.Headers(true)))
   .Build();
            var ordersController = client.OrdersController;
            OrdersCreateInput ordersCreateInput = new OrdersCreateInput
            {
                Body = new OrderRequest
                {
                    Intent = CheckoutPaymentIntent.Authorize,
                    PurchaseUnits = [ new PurchaseUnitRequest{
                Amount = new AmountWithBreakdown
                {
                    CurrencyCode = "usd",
                    MValue = "100",
                },
                ReferenceId = "AC5EA2C9-7E66-4E37-8340-4273AE354A92",//购买的项目id
                Description = "该订单仅包含基础服务，不包含（收入抵扣,所得税减免,已付税款）服务"
            },],
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
                string approveUrl = orderresult.Data.Links.First(link => link.Rel == "payer-action").Href;//支付链接
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
        /// <summary>
        /// 授权付款
        /// </summary>
        /// <param name="token">订单号</param>
        /// <param name="PayerID"></param>
        /// <returns></returns>
        public async Task<Response<string>> PaymentSuccess(string token, string PayerID)
        {
            string oAuthClientId = System.Configuration.ConfigurationManager.AppSettings["oAuthClientId"] + "";
            string oAuthClientSecret = System.Configuration.ConfigurationManager.AppSettings["oAuthClientSecret"] + "";
            PaypalServerSdkClient client = new PaypalServerSdkClient.Builder()
    .ClientCredentialsAuth(new ClientCredentialsAuthModel.Builder(oAuthClientId, oAuthClientSecret).Build())
    .Environment(PaypalServerSdk.Standard.Environment.Sandbox)
    .LoggingConfig(config => config
        .LogLevel(LogLevel.Information)
        .RequestConfig(reqConfig => reqConfig.Body(true))
        .ResponseConfig(respConfig => respConfig.Headers(true)))
    .Build();
            OrdersGetInput ordersGetInput = new OrdersGetInput
            {
                Id = token,
            };

            try
            {
                var ordersController = client.OrdersController;
                var paymentsController = client.PaymentsController;
                OrdersAuthorizeInput ordersAuthorizeInput = new OrdersAuthorizeInput
                {
                    Id = token,
                    Prefer = "return=minimal",
                };
                //获取授权付款id
                ApiResponse<OrderAuthorizeResponse> ordersresult = await ordersController.OrdersAuthorizeAsync(ordersAuthorizeInput);
                string Authorizationsid = ordersresult.Data.PurchaseUnits[0].Payments.Authorizations[0].Id;
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
                if (paystatus.Equals("Completed"))
                {
                    ApiResponse<Order> result = await ordersController.OrdersGetAsync(ordersGetInput);
                    AddAccountInformation(ordersresult.Data.Payer.EmailAddress, "PayPal", result.Data.Id, result.Data.PurchaseUnits[0].ReferenceId, result.Data.PurchaseUnits[0].Amount.MValue, result.Data.PurchaseUnits[0].Shipping.Name.FullName, result.Data.PurchaseUnits[0].Amount.CurrencyCode);
                }
                    return new Response<string>("", "");
            }
            catch (ApiException e)
            {
                // TODO: Handle exception here
                return new Response<string>(e.Message);
            }
        }
        /// <summary>
        /// 用户购买项目
        /// </summary>
        /// <param name="email">用户email</param>
        /// <param name="paymentplatform">用户支付平台</param>
        /// <param name="oid">用户支付平台订单号</param>
        /// <param name="rid">购买的项目id</param>
        /// <param name="currencycode">支付的货币类型</param>
        /// <param name="amount">购买的金额</param>
        /// <param name="fullname">用户名</param>
        public static void AddAccountInformation(string email,string paymentplatform,string oid,string rid,string amount,string fullname,string currencycode)
        {
            try
            {   
                int num = 1;
                string uid = "";
                string cmdText = "select top 1 id from Users  where account=@email OR email=@email";
                SqlCommand cmd = new SqlCommand(cmdText);
                cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = email;
                cmd.CommandText = cmdText;
                DataTable table = new DataTable();
                GoogleSqlDBHelper.ExecuteReader(cmd, table);//获取用户是否存在
                if(table == null || table.Rows.Count <= 0)
                {
                    //添加用户信息
                    if (string.IsNullOrEmpty(fullname))
                    {
                        fullname = email;
                    }
                    UsersModel usersModel = new UsersModel
                    {
                        Account = email,
                        Email = email,
                        Address = "",
                        Avatar = "",
                        Status = 1,
                        Access_token = "",
                        Expires_in = DateTime.Now,
                        Refiesh_token = "",
                        Mobilephone = "",
                        Birthdate = DateTime.Now,
                        Createdate = DateTime.Now,
                        Updatedate = DateTime.Now,
                        Domain = "",
                        Nickname = fullname,
                        Username = fullname,
                        Password = "",
                        Gender = 0,
                        SocialSecurityNumber = "",
                        CountryCode = "",
                        AdminArea1 = "",
                        AdminArea2 = "",
                        AddressLine1 = "",
                        Addressline2 = "",
                        PostalCode = ""
                    };
                    AccountService.CreateUser(usersModel);
                    if (num > 0 ){
                        table = new DataTable();
                        GoogleSqlDBHelper.ExecuteReader(cmd, table);//获取用户是否存在
                        uid = table.Rows[0]["id"] + "";
                    }
                }
                else
                {
                    uid = table.Rows[0]["id"] + "";
                }
                if(num > 0)
                {
                    //创建订单信息
                    UserOrderModel orderModel = new UserOrderModel
                    {
                        Projectid = rid,
                        OrderId = oid,
                        Uid = uid,
                        Account = email,
                        Amount = Pub.To<decimal>(amount),
                        PaymentPlatform = paymentplatform,
                        Createtime = DateTime.Now,
                        PayTime = DateTime.Now,
                        currencycode = currencycode
                    };
                    AccountService.AddAccountOrder(orderModel);
                    //创建用户购买项目
                    UserProjectModel projectModel = new UserProjectModel
                    {
                        uid = uid,
                        pid = rid,
                        CreateTime = DateTime.Now
                    };
                    AccountService.AddAccountProject(projectModel);
                }
            }
            catch (Exception ex)
            {
                Pub.SaveLog("PayService", $"创建用户账户信息出现错误，用户：{email}，错误信息：{ex.Message}");
            }
           
        }
    }
}
