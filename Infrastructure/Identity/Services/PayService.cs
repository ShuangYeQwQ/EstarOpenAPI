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
using Application.RequestModel;
using Google.Api;
//using iTextSharp.text.pdf.parser.clipper;
using System.Runtime.Serialization;

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
        public async Task<Response<string>> StripePaySuccessAsync(string uid, string PaymentIntentId)
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
                    AddAccountInformation(uid,billingDetails.Email,"Stripe", PaymentIntentId, charge.Metadata["ReferenceId"], (charge.Amount / 100).ToString(), charge.Currency,billingDetails.Name,"2023");
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
        /// <param name="signup_req">服务id</param>
        /// <returns></returns>
        public async Task<Response<string>> CreatePayPalSourceAsync(common_req<string> signup_req)
        {
            string moeny = "100.00";
            string desc = "该订单仅包含基础服务，不包含（收入抵扣,所得税减免,已付税款）服务";
            string serviceid = signup_req.actioninfo;
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
                    MValue = moeny,
                },
                ReferenceId = serviceid,//"AC5EA2C9-7E66-4E37-8340-4273AE354A92",//购买的项目id
                Description = desc
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
        public async Task<Response<string>> PaymentSuccess(string uid, string token, string PayerID)
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
                    AddAccountInformation(uid, ordersresult.Data.Payer.EmailAddress, "PayPal", result.Data.Id, result.Data.PurchaseUnits[0].ReferenceId, result.Data.PurchaseUnits[0].Amount.MValue, result.Data.PurchaseUnits[0].Shipping.Name.FullName, result.Data.PurchaseUnits[0].Amount.CurrencyCode, "2023");
                    return new Response<string>("支付失败！支付未完成");
                }
                    return new Response<string>("支付失败！支付未完成");
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
        /// <param name="rid">购买的服务id</param>
        /// <param name="currencycode">支付的货币类型</param>
        /// <param name="amount">购买的金额</param>
        /// <param name="fullname">用户名</param>
        /// /// <param name="year">服务年份</param>
        public static void AddAccountInformation(string uid, string email,string paymentplatform,string oid,string rid,string amount,string fullname,string currencycode,string year)
        {
            try
            {
                string cmdText = ""; 
                SqlCommand cmd;
                DataTable table;
                if (!string.IsNullOrEmpty(uid))
                {
                    cmdText = "select top 1 Uid from Users where account=@email OR email=@email";
                    cmd = new SqlCommand(cmdText);
                    cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = email;
                    cmd.CommandText = cmdText;
                    table = new DataTable();
                    GoogleSqlDBHelper.Fill(cmd, table);//获取用户是否存在
                    if (table == null || table.Rows.Count <= 0)
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
                            Status = 0,
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
                        table = new DataTable();
                        GoogleSqlDBHelper.Fill(cmd, table);//获取用户是否存在
                        uid = table.Rows[0]["id"] + "";
                    }
                    else
                    {
                        uid = table.Rows[0]["uid"] + "";
                    }
                }
                
                    //创建订单信息，订单先记录，后面修改服务id为用户服务id
                    UserOrderModel orderModel = new UserOrderModel
                    {
                        UserServiceId = rid,
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
                //将服务按服务模板点数/最近处理服务时间/最近处理服务状态/创建时间分配给员工，获取3个员工id，会计id
                string sid = "";
                string sid1 = "";
                string sid2 = "";
                cmdText = @" WITH MinPoints AS (
    SELECT MIN(s.ServiceModelPoints) AS MinPoints
    FROM staff s
    INNER JOIN Staff_Role r ON s.RoleId = r.Id
    WHERE r.RoleType IN (1, 2, 3)
),
RankedStaff AS (
    SELECT 
        s.Id, 
        r.RoleType, 
        s.ServiceModelPoints, 
        s.ProcessedServiceCount, 
        s.CreateDate, 
        s.HandleServiceDate,
        s.HandleServiceStatus,
        ROW_NUMBER() OVER (
            PARTITION BY r.RoleType
            ORDER BY 
                s.ServiceModelPoints, 
                s.HandleServiceDate ASC, 
                CASE 
                    WHEN s.HandleServiceStatus = 0 THEN 1
                    WHEN s.HandleServiceStatus = 3 THEN 2
                    ELSE 3
                END,
                s.CreateDate ASC
        ) AS RowNum
    FROM staff s
    INNER JOIN Staff_Role r ON s.RoleId = r.Id
    WHERE r.RoleType IN (1, 2, 3)
    AND s.ServiceModelPoints = (SELECT MinPoints FROM MinPoints)
    AND s.Status = '2'
)
SELECT Id, RoleType
FROM RankedStaff
WHERE RowNum = 1; ";
                     cmd = new SqlCommand(cmdText);
                     cmd.CommandText = cmdText;
                     table = new DataTable();
                    GoogleSqlDBHelper.Fill(cmd, table);
                    if (table != null && table.Rows.Count > 0)
                    {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        string id = table.Rows[i]["id"] + "";
                        string roleType = table.Rows[i]["RoleType"] + "";
                        switch (roleType)
                        {
                            case "1":
                                sid = id;
                                break;
                            case "2":
                                sid1 = id;
                                break;
                            case "3":
                                sid2 = id;
                                break;
                        }
                    }
                    }
                    //创建用户购买项目
                    UserServiceModel serviceModel = new UserServiceModel
                    {
                        UId = uid,
                        Name = "",
                        CreateTime = DateTime.Now,
                        BeginDate = DateTime.Now,
                        EndDate = DateTime.Now,
                        ServiceId = rid,
                        Status = "0",
                        Descs = "",
                        Year = year,
                        SId = sid,
                        SId1 = sid1,
                        SId2 = sid2,
                    };
                    int num = ServicesService.AddUserService(serviceModel);
                if(num > 0)
                {
                    cmdText = "select top 1 id from User_Service where Uid = '"+uid+"' AND Year = '"+year+"' AND ServiceId = '"+ rid + "' AND Status = '0' ";
                    string usid = GoogleSqlDBHelper.ExecuteScalar(cmdText);
                    if (string.IsNullOrEmpty(usid)){
                        //使用数据库模板，数据库添加员工选择的任务模板发布任务
                        string TaskTitle = "上传文件";
                        string TaskContent = "请上传您的报税资料相关文件，如（W-2）（1095）等相关的pdf或图片文件";
                        UserTaskModel userTaskModel = new UserTaskModel
                        {
                            Uid = uid,
                            CreateTime = DateTime.Now,
                            UpdateTime = DateTime.Now,
                            UserServiceId = usid,
                            Sid = "",
                            SendType = 0,
                            Status = 0,
                            Type = 0,
                            TaskTitle = TaskTitle,
                            TaskContent = TaskContent,
                        };
                        ServicesService.AddUserTask(userTaskModel);
                        //修改订单表数据服务id为用户服务id
                        cmdText = " update User_Orders set UserServiceId = '"+ usid + "' where OrderId = '"+ oid + "' and Uid = '"+uid+"' ";
                        string msg = "";
                        GoogleSqlDBHelper.ExecuteNonQuery(cmdText,ref msg);
                    }
                    

                }
                
            }
            catch (Exception ex)
            {
                Pub.SaveLog("PayService", $"创建用户账户信息出现错误，用户：{email}，错误信息：{ex.Message}");
            }
           
        }
    }
}
