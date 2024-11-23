using Application.Interfaces;
using Application.RequestModel;
using Application.RequestModel.AccountPage;
using Application.RequestModel.GoogleIdentityPlatform;
using Application.ResponseModel.GoogleIdentityPlatform;
using Application.ResponseModel.HomePage;
using Application.Wrappers;
using EStarGoogleCloud;
using Google.Type;
using GoogleCloudModel;
using Infrastructure.Identity.Models;
using Infrastructure.Shared;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DateTime = System.DateTime;

namespace Infrastructure.Identity.Services
{
    public class AccountService : IAccountService
    {
        /// <summary>
        /// 添加用户账号
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public static int AddAccount(UsersModel users)
        {
            int num = 0;
            string sql = "";
            sql = @"IF NOT EXISTS (SELECT 1 FROM Users WHERE Account = @Account OR Email = @Email OR Mobilephone = @Mobilephone)
BEGIN 
    INSERT INTO Users (Account, Password, UserName, CreateDate, UpdateDate, Access_Token, Expires_in, Refiesh_token, NickName, Gender, Avatar, Birthdate, Address, Status, Email, Mobilephone, Domain,SocialSecurityNumber,CountryCode,AdminArea1,AdminArea2,AddressLine1,Addressline2,PostalCode)
    VALUES (@Account, @Password, @UserName, @CreateDate, @UpdateDate, @Access_Token, @Expires_in, @Refiesh_token, @NickName, @Gender, @Avatar, @Birthdate, @Address, @Status, @Email, @Mobilephone, @Domain,@SocialSecurityNumber,@CountryCode,@AdminArea1,@AdminArea2,@AddressLine1,@Addressline2,@PostalCode)
END";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@Account", users.Account);
            cmd.Parameters.AddWithValue("@Password", users.Password);
            cmd.Parameters.AddWithValue("@UserName", users.Username);
            cmd.Parameters.AddWithValue("@CreateDate", users.Createdate);
            cmd.Parameters.AddWithValue("@UpdateDate", users.Updatedate);
            cmd.Parameters.AddWithValue("@Access_Token", users.Access_token);
            cmd.Parameters.AddWithValue("@Expires_in", users.Expires_in);
            cmd.Parameters.AddWithValue("@Refiesh_token", users.Refiesh_token);
            cmd.Parameters.AddWithValue("@NickName", users.Nickname);
            cmd.Parameters.AddWithValue("@Gender", users.Gender);
            cmd.Parameters.AddWithValue("@Avatar", users.Avatar);
            cmd.Parameters.AddWithValue("@Birthdate", users.Birthdate);
            cmd.Parameters.AddWithValue("@Address", users.Address);
            cmd.Parameters.AddWithValue("@Status", users.Status);
            cmd.Parameters.AddWithValue("@Email", users.Email);
            cmd.Parameters.AddWithValue("@Mobilephone", users.Mobilephone);
            cmd.Parameters.AddWithValue("@Domain", users.Domain);
            cmd.Parameters.AddWithValue("@SocialSecurityNumber", users.SocialSecurityNumber);
            cmd.Parameters.AddWithValue("@CountryCode", users.CountryCode);
            cmd.Parameters.AddWithValue("@AdminArea1", users.AdminArea1);
            cmd.Parameters.AddWithValue("@AdminArea2", users.AdminArea2);
            cmd.Parameters.AddWithValue("@AddressLine1", users.AddressLine1);
            cmd.Parameters.AddWithValue("@Addressline2", users.Addressline2);
            cmd.Parameters.AddWithValue("@PostalCode", users.PostalCode);
            try
            {
                num = GoogleSqlDBHelper.ExecuteNonQuery(cmd);
                if (num <= 0)
                {
                    string logSql = Pub.ReplaceSqlParameters(sql, cmd);
                    Pub.SaveLog(nameof(AccountService), $"新增账号信息失败,SQL: {logSql}");
                }
            }
            catch (Exception ex)
            {
                string logSql = Pub.ReplaceSqlParameters(sql, cmd);
                Pub.SaveLog(nameof(AccountService), $"插入帐户时发生异常:{ex.Message} , SQL: {logSql}");
            }
            return num;
        }

        /// <summary>
        /// 添加用户购买的服务
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public static void AddAccountProject(UserProjectModel userProjectModel)
        {
            int num = 0;
            string sql = "";
            sql = @" insert into user_project(uid,pid,createtime) values(@uid,@pid,@createtime) ";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@uid", userProjectModel.uid);
            cmd.Parameters.AddWithValue("@pid", userProjectModel.pid);
            cmd.Parameters.AddWithValue("@createtime", userProjectModel.CreateTime);
            try
            {
                num = GoogleSqlDBHelper.ExecuteNonQuery(cmd);
                if (num <= 0)
                {
                    string logSql = Pub.ReplaceSqlParameters(sql, cmd);
                    Pub.SaveLog(nameof(AccountService), $"新增用户购买的服务失败, SQL: {logSql}");
                }
            }
            catch (Exception ex)
            {
                string logSql = Pub.ReplaceSqlParameters(sql, cmd);
                Pub.SaveLog(nameof(AccountService), $"插入用户购买的服务时发生异常:{ex.Message} , SQL: {logSql}");
            }
        }
        /// <summary>
        /// 添加用户订单
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public static void AddAccountOrder(UserOrderModel orderModel)
        {
            int num = 0;
            string sql = "";
            sql = @" insert into orders(uid,pid,createtime,paymentplatform,paytime,orderid,amount,account,currencycode) values(
@Uid,@Projectid,@Createtime,@PaymentPlatform,@PayTime,@OrderId,@Amount,@Account,@currencycode) ";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@Projectid", orderModel.Projectid);
            cmd.Parameters.AddWithValue("@OrderId", orderModel.OrderId);
            cmd.Parameters.AddWithValue("@Uid", orderModel.Uid);
            cmd.Parameters.AddWithValue("@Account", orderModel.Account);
            cmd.Parameters.AddWithValue("@Amount", orderModel.Amount);
            cmd.Parameters.AddWithValue("@PaymentPlatform", orderModel.PaymentPlatform);
            cmd.Parameters.AddWithValue("@Createtime", orderModel.Createtime);
            cmd.Parameters.AddWithValue("@PayTime", orderModel.PayTime);
            cmd.Parameters.AddWithValue("@currencycode", orderModel.currencycode);
            try
            {
                num = GoogleSqlDBHelper.ExecuteNonQuery(cmd);
                if (num <= 0)
                {
                    string logSql = Pub.ReplaceSqlParameters(sql, cmd);
                    Pub.SaveLog(nameof(AccountService), $"新增用户订单失败, SQL: {logSql}");
                }
            }
            catch (Exception ex)
            {
                string logSql = Pub.ReplaceSqlParameters(sql, cmd);
                Pub.SaveLog(nameof(AccountService), $"插入用户订单时发生异常:{ex.Message} , SQL: {logSql}");
            }
        }

        /// <summary>
        /// 添加用户银行账号信息
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public static void AddAccountInformation(UsersModel users)
        {
            int num = 0;
            string sql = "";
            sql = @"";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@Account", users.Account);
            cmd.Parameters.AddWithValue("@Password", users.Password);
            try
            {
                num = GoogleSqlDBHelper.ExecuteNonQuery(cmd);
                if (num <= 0)
                {
                    string logSql = Pub.ReplaceSqlParameters(sql, cmd);
                    Pub.SaveLog(nameof(AccountService), $"新增用户银行账号信息失败, SQL: {logSql}");
                }
            }
            catch (Exception ex)
            {
                string logSql = Pub.ReplaceSqlParameters(sql, cmd);
                Pub.SaveLog(nameof(AccountService), $"插入用户银行账号信息时发生异常:{ex.Message} , SQL: {logSql}");
            }
        }

        /// <summary>
        /// 添加用户基本信息
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        public async Task<Response<string>> CreateUserInformationAsync(common_req<UserInformation_req> signup_req)
        {
            
            int num = 0;
            string sql = "";
            UserInformation_req userInformation_Req = signup_req.actioninfo;
            string cmdText = "select top 1 id from Users  where account=@email OR email=@email OR MobilePhone = @MobilePhone";
            SqlCommand cmd = new SqlCommand(cmdText);
            cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = userInformation_Req.Email;
            cmd.Parameters.Add("@MobilePhone", SqlDbType.NVarChar).Value = userInformation_Req.Phone;
            cmd.CommandText = cmdText;
            DataTable table = new DataTable();
            GoogleSqlDBHelper.ExecuteReader(cmd, table);//获取用户是否存在
            if (table == null || table.Rows.Count <= 0)
            {
                List<string> addressParts = new List<string>();

                if (!string.IsNullOrEmpty(userInformation_Req.Country))
                    addressParts.Add(userInformation_Req.Country);

                if (!string.IsNullOrEmpty(userInformation_Req.Province))
                    addressParts.Add(userInformation_Req.Province);

                if (!string.IsNullOrEmpty(userInformation_Req.City))
                    addressParts.Add(userInformation_Req.City);

                if (!string.IsNullOrEmpty(userInformation_Req.Streetaddress))
                    addressParts.Add(userInformation_Req.Streetaddress);

                if (!string.IsNullOrEmpty(userInformation_Req.Apartmentnumber))
                    addressParts.Add(userInformation_Req.Apartmentnumber);

                // 使用 "/" 连接所有非空部分
                string address = string.Join("/", addressParts);
                UsersModel usersModel = new UsersModel
                {
                    Account = userInformation_Req.Email,
                    Email = userInformation_Req.Email,
                    Address = address,
                    Avatar = "",
                    Status = 1,
                    Access_token = "",
                    Expires_in = DateTime.Now,
                    Refiesh_token = "",
                    Mobilephone = userInformation_Req.Phone,
                    Birthdate = Pub.To<DateTime>(userInformation_Req.Birth),
                    Createdate = DateTime.Now,
                    Updatedate = DateTime.Now,
                    Domain = "",
                    Nickname = userInformation_Req.Email,
                    Username = userInformation_Req.Name,
                    Password = "",
                    Gender = 0,
                    SocialSecurityNumber = userInformation_Req.Socialsecuritynumber,
                    CountryCode = userInformation_Req.Country,
                    AdminArea1 = userInformation_Req.Province,
                    AdminArea2 = userInformation_Req.City,
                    AddressLine1 = userInformation_Req.Streetaddress,
                    Addressline2 = userInformation_Req.Apartmentnumber,
                    PostalCode = ""
                };
                AccountService.CreateUser(usersModel);
                return new Response<string>("", "");
            }
            else
            {
                sql = @"UPDATE Users SET  UserName = @UserName,Brithdate = @Brithdate,MobilePhone = @MobilePhone,SocialSecurityNumber = @SocialSecurityNumber,
CountryCode = @CountryCode,AdminArea1 = @AdminArea1,AdminArea2 = @AdminArea2,AddressLine1 = @AddressLine1,Addressline2 = @Addressline2
WHERE Email = @Email OR MobilePhone = @MobilePhone";
                cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@UserName", userInformation_Req.Name);
                cmd.Parameters.AddWithValue("@Email", userInformation_Req.Email);
                cmd.Parameters.AddWithValue("@MobilePhone", userInformation_Req.Phone);
                cmd.Parameters.AddWithValue("@Brithdate", userInformation_Req.Birth);
                cmd.Parameters.AddWithValue("@SocialSecurityNumber", userInformation_Req.Socialsecuritynumber);
                cmd.Parameters.AddWithValue("@CountryCode", userInformation_Req.Country);
                cmd.Parameters.AddWithValue("@AdminArea1", userInformation_Req.Province);
                cmd.Parameters.AddWithValue("@AdminArea2", userInformation_Req.City);
                cmd.Parameters.AddWithValue("@AddressLine1", userInformation_Req.Streetaddress);
                cmd.Parameters.AddWithValue("@AddressLine2", userInformation_Req.Apartmentnumber);
                try
                {
                    num = GoogleSqlDBHelper.ExecuteNonQuery(cmd);
                    if (num <= 0)
                    {
                        string logSql = Pub.ReplaceSqlParameters(sql, cmd);
                        Pub.SaveLog(nameof(AccountService), $"添加用户基本信息失败, SQL: {logSql}");
                    }
                    return new Response<string>("", "");
                }
                catch (Exception ex)
                {
                    string logSql = Pub.ReplaceSqlParameters(sql, cmd);
                    Pub.SaveLog(nameof(AccountService), $"添加用户基本信息时发生异常:{ex.Message} , SQL: {logSql}");
                    return new Response<string>($"添加用户基本信息时发生异常:{ex.Message}");
                }
            }
        }
        /// <summary>
        /// 创建用户并发送账户，密码
        /// </summary>
        /// <param name="users"></param>
        public static async void CreateUser(UsersModel users)
        {
            //
            string subject = "";
            string body = "";
            string pwd = users.Password;
            try
            {
                if (string.IsNullOrEmpty(pwd))
                {
                    pwd = Pub.GeneratePassword(8);
                    users.Password = Pub.HashPassword(pwd);
                }
                int num = AccountService.AddAccount(users);
                if (num > 0)
                {
                     AddGoogleUserAsync(users.Email, pwd, users.Mobilephone);
                    DataTable table = new DataTable();
                    //发送用户账户信息邮件
                    string cmdText = "select top 2 EmailSubject,EmailBody from email_config  where EmailType = 1 or EmailType = 2 ";
                    SqlCommand cmd = new SqlCommand(cmdText)
                    {
                        CommandText = cmdText
                    };
                    table = new DataTable();
                    GoogleSqlDBHelper.ExecuteReader(cmd, table);//获取需要发送的邮件信息
                    if (table != null && table.Rows.Count > 0)
                    {
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            subject = table.Rows[i]["EmailSubject"] + "";
                            body = table.Rows[i]["EmailBody"] + "";
                            string newBody = body.Replace("%UserEmail%", users.Email);
                            newBody = newBody.Replace("%UserPassWord%", pwd);
                            //发送账户信息
                            Pub.SendEmail(users.Email, subject, newBody);
                            if (table.Rows.Count >= 2)
                            {
                                System.Threading.Thread.Sleep(1000);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            
        }
        /// <summary>
        /// 创建用户googleIdentity Platform账户
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="phone"></param>
        public static async void AddGoogleUserAsync(string email,string password,string phone)
        {
            try
            {
                phone = "+8612949583948";
                string apiKey = System.Configuration.ConfigurationManager.AppSettings["IdentityKey"] + "";
                using var client = new HttpClient();
                var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={apiKey}";
                var requestData = new GooglesignUp_req();
                requestData.email = email;
                requestData.password = password;
                requestData.returnSecureToken = true;
                if (!string.IsNullOrEmpty(phone))
                {
                    requestData.PhoneNumber = phone;
                }


                var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Pub.SaveLog(nameof(AccountService), $"Failed to create user: {error}");
                    //throw new HttpRequestException();
                }

                var result = await response.Content.ReadAsStringAsync();
                var singupresponse = (GooglesignUp_res)JsonConvert.DeserializeObject(result, typeof(GooglesignUp_res));
                //var singupresponse = JsonSerializer.Deserialize<GooglesignUp_res>(result);
                await SendEmailVerificationAsync(singupresponse.idToken);
                Pub.SaveLog(nameof(AccountService), result);
                //return result; // 返回包含用户的 `idToken` 和 `localId` 等信息
            }
            catch (Exception ex)
            {
                Pub.SaveLog(nameof(AccountService), $"Failed to create user: {ex.Message}");
            }
           
        }
        /// <summary>
        /// 绑定Google Identity Platform账户email
        /// </summary>
        /// <param name="idToken"></param>
        /// <returns></returns>
        public static async Task SendEmailVerificationAsync(string idToken)
        {
            try
            {
                string apiKey = System.Configuration.ConfigurationManager.AppSettings["IdentityKey"] + "";
                using var client = new HttpClient();
                var url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={apiKey}";

                var requestData = new
                {
                    requestType = "VERIFY_EMAIL",
                    idToken = idToken
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Failed to send email verification: {error}");
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        /// <summary>
        /// 查看用户Google Identity Platform账户email是否验证
        /// </summary>
        /// <param name="idToken"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<bool> CheckEmailVerifiedAsync(string idToken)
        {
            string apiKey = System.Configuration.ConfigurationManager.AppSettings["IdentityKey"] + "";
            using var client = new HttpClient();
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:lookup?key={apiKey}";

            var requestData = new { idToken };
            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to lookup account: {error}");
            }

            var responseData = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            var isVerified = responseData?.users[0]?.emailVerified == true;

            return isVerified;
        }
        /// <summary>
        /// 发送用户Google Identity Platform账户手机验证码
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<string> SendPhoneVerificationCodeAsync(string phoneNumber)
        {
            string apiKey = System.Configuration.ConfigurationManager.AppSettings["IdentityKey"] + "";
            using var client = new HttpClient();
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendVerificationCode?key={apiKey}";

            var requestData = new
            {
                phoneNumber = phoneNumber,
                recaptchaToken = "RECAPTCHA_TOKEN" // 可选：用于防止滥用
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to send phone verification code: {error}");
            }

            var responseData = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            var sessionInfo = responseData?.sessionInfo;

            return sessionInfo; // 返回用于后续验证的 session 信息
        }
        /// <summary>
        /// 验证用户Google Identity Platform账户手机验证码
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task BindPhoneNumberAsync(string sessionInfo, string verificationCode)
        {
            string apiKey = System.Configuration.ConfigurationManager.AppSettings["IdentityKey"] + "";
            using var client = new HttpClient();
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPhoneNumber?key={apiKey}";

            var requestData = new
            {
                sessionInfo = sessionInfo,
                code = verificationCode
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to bind phone number: {error}");
            }

            var responseData = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            var phoneNumber = responseData?.phoneNumber;

            Console.WriteLine($"Phone number {phoneNumber} successfully bound to user.");
        }
        /// <summary>
        /// 更新用户手机号到 Firebase
        /// </summary>
        /// <param name="idToken"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task UpdatePhoneNumberAsync(string idToken, string phoneNumber)
        {
            string apiKey = System.Configuration.ConfigurationManager.AppSettings["IdentityKey"] + "";
            using var client = new HttpClient();
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:update?key={apiKey}";

            var requestData = new
            {
                idToken = idToken,
                phoneNumber = phoneNumber
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to update phone number: {error}");
            }

            Console.WriteLine("Phone number updated successfully.");
        }
    }
    
}
