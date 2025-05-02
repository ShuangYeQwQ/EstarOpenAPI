using Application.Interfaces;
using Application.RequestModel;
//using iTextSharp.text.pdf.parser.clipper;
using Application.RequestModel.PayPage;
using Application.ResponseModel.PayPage;
using Application.Wrappers;
using EStarGoogleCloud;
using Infrastructure.Identity.Models;
using Infrastructure.Shared;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using PaypalServerSdk.Standard;
using PaypalServerSdk.Standard.Authentication;
using PaypalServerSdk.Standard.Exceptions;
using PaypalServerSdk.Standard.Http.Response;
using PaypalServerSdk.Standard.Models;
using Stripe;
using Stripe.V2;
using System.Data;
using System.Data.Common;

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
        public async Task<Response<string>> StripePaySuccessAsync(common_req<PayPayPayment_req> signup_req)//string uid, string PaymentIntentId
        {
            string uid = signup_req.User;
            string oid = signup_req.Actioninfo.Oid;
            if (string.IsNullOrEmpty(oid))
            {
                return new Response<string>("PaymentIntentId is required");
            }

            try
            {
                // 使用 Stripe SDK 查询 PaymentIntent
                var service = new PaymentIntentService();
                var paymentIntent = service.Get(oid);

                if (paymentIntent.Status == "succeeded")
                {
                    // 支付成功，记录支付信息
                    var chargeService = new ChargeService();
                    var charge = chargeService.Get(paymentIntent.LatestChargeId);
                    var billingDetails = charge.BillingDetails;
                    AddAccountInformation(uid,billingDetails.Email,"Stripe", oid, charge.Metadata["ReferenceId"], (charge.Amount / 100).ToString(), charge.Currency,billingDetails.Name, signup_req.Actioninfo.PayItems);
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
        public async Task<Response<int>> CreatePayPalSourceAsync(common_req<PayPal_req> signup_req)
        {
            string moeny = signup_req.Actioninfo.TotalMoney; //"100.00";
            string desc = signup_req.Actioninfo.PayDesc;
            string serviceid = signup_req.Actioninfo.ServiceId;
            string paytype = signup_req.Actioninfo.PayType;
            //验证是否需要付费
            if (paytype.Equals("1") && !string.IsNullOrEmpty(signup_req.User))
            {
                string cmdtxt = string.Format(@" select top 1 id from User_Orders where uid = '{0}' ", signup_req.User);
                string oid =  GoogleSqlDBHelper.ExecuteScalar(cmdtxt);
                if (!string.IsNullOrEmpty(oid)) {
                    string msg = ServicesService.AddUserService(signup_req.User, serviceid, "0", "PayPal","","", signup_req.Actioninfo.PayItems);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        return new Response<int>(102, "购买失败！创建用户服务时出现问题："+msg);
                    }
                    return new Response<int>(101,"");
                }
            }


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
            List<Item> items = new List<Item>();
            for (int i = 0; i < signup_req.Actioninfo.PayItems.Count; i++)
            {
                Item item = new Item();
                item.Name = signup_req.Actioninfo.PayItems[i].Name;
                item.UnitAmount = new Money { CurrencyCode = "usd", MValue = signup_req.Actioninfo.PayItems[i].Money };
                item.Quantity = signup_req.Actioninfo.PayItems[i].Num;
                //item.Description = signup_req.Actioninfo.PayItems[i].Description;
                items.Add(item);
            }
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
                    Breakdown = new AmountBreakdown {
                        ItemTotal = new Money {
                    CurrencyCode = "usd",
                    MValue = moeny,
                        }
                    }
                },
                ReferenceId = serviceid,//"AC5EA2C9-7E66-4E37-8340-4273AE354A92",//购买的项目id
                Description = desc,
               Items =  items,
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
                return new Response<int>(100,approveUrl);
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
                return new Response<int>(102,e.Message);
            }
        }
        /// <summary>
        /// 授权付款
        /// </summary> string uid, string token, string PayerID
        /// <returns></returns>
        public async Task<Response<string>> PaymentSuccess(common_req<PayPayPayment_req> signup_req)
        {
            string uid = signup_req.User;
            string oid = signup_req.Actioninfo.Oid;
            //string serviceyear = signup_req.Actioninfo.BeginServiceDate;
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
                Id = oid,
            };

            try
            {
                var ordersController = client.OrdersController;
                var paymentsController = client.PaymentsController;
                OrdersAuthorizeInput ordersAuthorizeInput = new OrdersAuthorizeInput
                {
                    Id = oid,
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
                    //string uid, string email,string paymentplatform,string oid,string rid,string amount,string fullname,string currencycode,string year
                    string email = ordersresult.Data.Payer.EmailAddress;
                    string serviceid = result.Data.PurchaseUnits[0].ReferenceId;
                    string amount = result.Data.PurchaseUnits[0].Amount.MValue;
                    string fullname = result.Data.PurchaseUnits[0].Shipping.Name.FullName;
                    string currencycode = result.Data.PurchaseUnits[0].Amount.CurrencyCode; 
                    string msg =  AddAccountInformation(uid, email, "PayPal",oid , serviceid, amount, fullname,currencycode, signup_req.Actioninfo.PayItems);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        return new Response<string>("支付成功！创建用户服务信息失败："+msg);
                    }
                    return new Response<string>("支付成功！","");
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
        public static string AddAccountInformation(string uid, string email,string paymentplatform,string oid,string rid,string amount,string fullname,string currencycode, List<PayItem> payItems)
        {
            try
            {
                string cmdText = ""; 
                SqlCommand cmd;
                DataTable table;
                //用户不存在创建
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
                            AccessToken = "",
                            ExpiresIn = DateTime.Now,
                            RefieshToken = "",
                            Mobilephone = "",
                            Birthdate = DateTime.Now,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            Domain = "",
                            NickName = fullname,
                            UserName = fullname,
                            Password = "",
                            Gender = 0,
                            SocialSecurityNumber = "",
                            CountryCode = "",
                            AdminArea1 = "",
                            AdminArea2 = "",
                            AddressLine1 = "",
                            Addressline2 = "",
                            PostalCode = "",
                            Balance = "0",
                            GooglelocalId = "",
                            EmailVerification = "0",
                            PhoneVerification = "0",
                            FirstBuy = "0"
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
                string msg = ServicesService.AddUserService(uid, rid, amount, paymentplatform, oid, currencycode, payItems);
                if(!string.IsNullOrEmpty(msg))
                {
                    return "添加用户服务失败！"+msg;
                }
                return "";
                //string usid = "";
                //if (num > 0)
                //{
                //    cmdText = "select top 1 id from User_Service where Uid = '"+uid+ "'  AND ServiceId = '"+ rid + "' AND Status = '0' ";//AND ServiceBeginYear = '" + year+"'
                //     usid = GoogleSqlDBHelper.ExecuteScalar(cmdText);
                //    if (string.IsNullOrEmpty(usid)){
                //        //使用数据库模板，数据库添加员工选择的任务模板发布任务
                //        string TaskTitle = "上传文件";
                //        string TaskContent = "请上传您的报税资料相关文件，如（W-2）（1095）等相关的pdf或图片文件";
                //        UserTaskModel userTaskModel = new UserTaskModel
                //        {
                //            Uid = uid,
                //            CreateTime = DateTime.Now,
                //            UpdateTime = DateTime.Now,
                //            UserServiceId = usid,
                //            Sid = "",
                //            SendType = 0,
                //            Status = 0,
                //            Type = 0,
                //            TaskTitle = TaskTitle,
                //            TaskContent = TaskContent,
                //        };
                //        ServicesService.AddUserTask(userTaskModel);
                //        //修改订单表数据服务id为用户服务id
                //       // cmdText = " update User_Orders set UserServiceId = '"+ usid + "' where OrderId = '"+ oid + "' and Uid = '"+uid+"' ";
                //       // string msg = "";
                //       // GoogleSqlDBHelper.ExecuteNonQuery(cmdText,ref msg);
                //    }
                //}

            }
            catch (Exception ex)
            {
                Pub.SaveLog("PayService", $"创建用户账户信息出现错误，用户：{email}，错误信息：{ex.Message}");
                return ex.Message;
            }
           
        }




       
    }
}
