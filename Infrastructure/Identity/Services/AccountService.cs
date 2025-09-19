using Application.Interfaces;
using Application.RequestModel;
using Application.RequestModel.AccountPage;
using Application.RequestModel.GoogleIdentityPlatform;
using Application.ResponseModel.AccountPage;
using Application.ResponseModel.GoogleIdentityPlatform;
using Application.ResponseModel.ServicePage;
using Application.Wrappers;
using EStarGoogleCloud;
using Infrastructure.Identity.Models;
using Infrastructure.Shared;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;
using System.Management;
using System.Text;
using static Application.ResponseModel.AccountPage.UserInformation_res;
using DateTime = System.DateTime;

namespace Infrastructure.Identity.Services
{
    public class AccountService : IAccountService
    {
        private static readonly string apiKey = System.Configuration.ConfigurationManager.AppSettings["IdentityKey"] + "";
        /// <summary>
        /// 添加用户账号
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public static int AddAccount(UsersModel users)
        {
            int num = 0;
            string sql = "";
            sql = @"IF NOT EXISTS (SELECT top 1 FROM Users WHERE Account = @Account OR Email = @Email OR Mobilephone = @Mobilephone)
BEGIN 
    INSERT INTO Users (Account, Password, UserName, CreateDate, UpdateDate, AccessToken, ExpiresIn, RefieshToken, NickName, Gender, Avatar, Birthdate, Address, Status, Email, Mobilephone,SocialSecurityNumber,CountryCode,AdminArea1,AdminArea2,AddressLine1,Addressline2,PostalCode,Domain,Balance,GooglelocalId,EmailVerification,PhoneVerification,FirstBuy)
    VALUES (@Account, @Password, @UserName, @CreateDate, @UpdateDate, @AccessToken, @ExpiresIn, @RefieshToken, @NickName, @Gender, @Avatar, @Birthdate, @Address, @Status, @Email, @Mobilephone,@SocialSecurityNumber,@CountryCode,@AdminArea1,@AdminArea2,@AddressLine1,@Addressline2,@PostalCode,@Domain,@Balance,@GooglelocalId,@EmailVerification,@PhoneVerification,@FirstBuy)
END";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@Account", users.Account);
            cmd.Parameters.AddWithValue("@Password", users.Password);
            cmd.Parameters.AddWithValue("@UserName", users.UserName);
            cmd.Parameters.AddWithValue("@CreateDate", users.CreateDate);
            cmd.Parameters.AddWithValue("@UpdateDate", users.UpdateDate);
            cmd.Parameters.AddWithValue("@AccessToken", users.AccessToken);
            cmd.Parameters.AddWithValue("@ExpiresIn", users.ExpiresIn);
            cmd.Parameters.AddWithValue("@RefieshToken", users.RefieshToken);
            cmd.Parameters.AddWithValue("@NickName", users.NickName);
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
            cmd.Parameters.AddWithValue("@Balance", users.Balance);
            cmd.Parameters.AddWithValue("@GooglelocalId", users.GooglelocalId);
            cmd.Parameters.AddWithValue("@EmailVerification", users.EmailVerification);
            cmd.Parameters.AddWithValue("@PhoneVerification", users.PhoneVerification);
            cmd.Parameters.AddWithValue("@FirstBuy", users.FirstBuy);
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
            //AddGoogleUserAsync(signup_req.actioninfo.Email, "GZi9VNAc", signup_req.actioninfo.Phone);
            UserInformation_req userInformation_Req = signup_req.Actioninfo;
            int num = 0;
            string sql = "";
            string cmdText = "select top 1 uid from Users  where account=@email OR email=@email OR MobilePhone = @MobilePhone";
            SqlCommand cmd = new SqlCommand(cmdText);
            cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = userInformation_Req.Email;
            cmd.Parameters.Add("@MobilePhone", SqlDbType.NVarChar).Value = userInformation_Req.Phone;
            cmd.CommandText = cmdText;
            DataTable table = new DataTable();
            GoogleSqlDBHelper.Fill(cmd, table);//获取用户是否存在
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
                    AccessToken = "",
                    ExpiresIn = DateTime.Now,
                    RefieshToken = "",
                    Mobilephone = userInformation_Req.Phone,
                    Birthdate = Pub.To<DateTime>(userInformation_Req.Birth),
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Domain = "",
                    NickName = userInformation_Req.Email,
                    UserName = userInformation_Req.Name,
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
                //查看是否绑定email/手机
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
                //生成密码
                if (string.IsNullOrEmpty(pwd))
                {
                    pwd = Pub.GeneratePassword(8);
                    users.Password = Pub.HashPassword(pwd);
                }
                //添加账户
                int num = AccountService.AddAccount(users);
                if (num > 0)
                {
                    DataTable table = new DataTable();
                    //发送用户账户信息邮件
                    string cmdText = "select top 2 EmailSubject,EmailBody from EmailConfig  where EmailType = 1 or EmailType = 2 order by EmailType ";
                    SqlCommand cmd = new SqlCommand(cmdText)
                    {
                        CommandText = cmdText
                    };
                    table = new DataTable();
                    GoogleSqlDBHelper.Fill(cmd, table);//获取需要发送的邮件信息
                    if (table != null && table.Rows.Count > 0)
                    {
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            subject = table.Rows[i]["EmailSubject"] + "";
                            body = table.Rows[i]["EmailBody"] + "";
                            string newBody = body.Replace("{UserEmail}", users.Email);
                            newBody = newBody.Replace("{UserPassWord}", pwd);
                            newBody = newBody.Replace("{UserName}", users.UserName);
                            //发送账户信息
                            await Task.Run(() => Pub.SendEmail(users.Email, subject, newBody));
                            //if (table.Rows.Count >= 2)
                            //{
                            //    System.Threading.Thread.Sleep(1000);
                            //}
                        }
                    }
                    AddGoogleUserAsync(users.Email, pwd, users.Mobilephone);
                }
            }
            catch (Exception ex)
            {
                Pub.SaveLog(nameof(AccountService), $"创建账户时发生错误：{ex.Message}");
            }

        }
        /// <summary>
        /// 创建用户googleIdentity Platform账户
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="phone"></param>
        public static async void AddGoogleUserAsync(string email, string password, string phone)
        {
            try
            {
               
                using var client = new HttpClient();
                var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={apiKey}";
                var requestData = new GooglesignUp_req();
                requestData.email = email;
                requestData.PhoneNumber = phone;
                requestData.password = password;
                requestData.returnSecureToken = true;
                string idToken = "";
                if (!string.IsNullOrEmpty(phone))
                {
                    requestData.PhoneNumber = phone;
                }
                var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    Pub.SaveLog(nameof(AccountService), $"Failed to create user: {result}");
                    return;
                    ////throw new HttpRequestException();
                }
                var singupresponse = (GooglesignUp_res)JsonConvert.DeserializeObject(result, typeof(GooglesignUp_res));
                idToken = singupresponse.idToken;
                string cmdText = " update users set GooglelocalId = '"+ singupresponse.localId+ "' where email = '"+ email + "' ";
                SqlCommand cmd = new SqlCommand(cmdText)
                {
                    CommandText = cmdText
                };
                GoogleSqlDBHelper.ExecuteNonQuery(cmd);
                //await SendEmailVerificationAsync(idToken);
                //await CheckEmailVerifiedAsync(idToken);
            }
            catch (Exception ex)
            {
                Pub.SaveLog(nameof(AccountService), $"Failed to create user: {ex.Message}");
                return;
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
                //string apiKey = System.Configuration.ConfigurationManager.AppSettings["IdentityKey"] + "";
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
                }
            }
            catch (Exception ex)
            {
                Pub.SaveLog(nameof(AccountService), $"MFA 启用失败: {ex.Message}");
            }
        }
        /// <summary>
        /// 查看用户Google Identity Platform账户
        /// </summary>
        /// <param name="idToken"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public static async Task<bool> CheckEmailVerifiedAsync(string localId)
        {
            try
            {
                //string apiKey = System.Configuration.ConfigurationManager.AppSettings["IdentityKey"] + "";
                string email = "shuangyeqwq@gmail.com";
               
                using var client = new HttpClient();
                var url = $"https://identitytoolkit.googleapis.com/v1/accounts:lookup?key={apiKey}";
                // 构造请求数据
                var requestData = new
                {
                    idToken = localId // localId 列表
                };
                // 将请求数据序列化为 JSON
                var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

                // 发送 POST 请求
                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return false;
                    ////throw new HttpRequestException($"Failed to lookup account: {error}");
                }

                var responseData = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                var isVerified = responseData?.users[0]?.emailVerified == true;//获取email是否验证
                return isVerified;
            }
            catch (Exception ex)
            {
                Pub.SaveLog(nameof(AccountService), $"MFA 启用失败: {ex.Message}");
                return false;
            }

        }
        /// <summary>
        /// 启用Google Identity Platform mfa第一步，绑定手机
        /// </summary>
        /// <param name="idToken"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static async Task StartmfaEnrollmentAsync(string idToken, string phone)
        {
            try
            {
                bool isemail = await CheckEmailVerifiedAsync(idToken);
                if (isemail)
                {
                    //string apiKey = System.Configuration.ConfigurationManager.AppSettings["IdentityKey"] + "";
                    var requestBody = new
                    {
                        idToken = idToken,
                        phoneEnrollmentInfo = new
                        {
                            phoneNumber = phone, // 用户手机号

                        }
                    };
                    using var client = new HttpClient();
                    var mfaUrl = $"https://identitytoolkit.googleapis.com/v2/accounts/mfaEnrollment:start?key={apiKey}";
                    var mfaContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                    var mfaResponse = await client.PostAsync(mfaUrl, mfaContent);
                    var responseDatas = await mfaResponse.Content.ReadAsStringAsync();
                    var resultss = JsonConvert.DeserializeObject<dynamic>(responseDatas);
                    if (mfaResponse.IsSuccessStatusCode)
                    {
                        string sess = resultss.phoneSessionInfo?.sessionInfo + "";
                        return;
                    }
                    else
                    {
                        Pub.SaveLog(nameof(AccountService), $"MFA 启用失败: {await mfaResponse.Content.ReadAsStringAsync()}");
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                Pub.SaveLog(nameof(AccountService), $"MFA 启用失败: {ex.Message}");
            }

        }
        /// <summary>
        /// 启用Google Identity Platform mfa第二步，验证码验证
        /// </summary>
        /// <param name="idToken"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static async Task StartmfaEnrollmentAsync(string idtoken, string sessioninfo, string code)
        {
            try
            {
                //string apiKey = System.Configuration.ConfigurationManager.AppSettings["IdentityKey"] + "";
                using var client = new HttpClient();
                var requestBody2 = new
                {
                    idToken = idtoken, // 从登录获取的 ID Token
                                       //phoneNumber = phone,
                    phoneVerificationInfo = new
                    {
                        sessionInfo = sessioninfo,
                        code = code,

                    }
                };
                var mfaUrl = $"https://identitytoolkit.googleapis.com/v2/accounts/mfaEnrollment:finalize?key={apiKey}";
                var mfaContent = new StringContent(JsonConvert.SerializeObject(requestBody2), Encoding.UTF8, "application/json");
                var mfaResponse = await client.PostAsync(mfaUrl, mfaContent);
                var responseDatas2 = await mfaResponse.Content.ReadAsStringAsync();
                var resultss2 = JsonConvert.DeserializeObject<dynamic>(responseDatas2);
                if (mfaResponse.IsSuccessStatusCode)
                {
                    Pub.SaveLog(nameof(AccountService), $"Session Info: {resultss2}");
                    Console.WriteLine("MFA 启用成功！");
                }
                else
                {
                    Console.WriteLine($"MFA 启用失败: {await mfaResponse.Content.ReadAsStringAsync()}");
                }

            }
            catch (Exception ex)
            {
                Pub.SaveLog(nameof(AccountService), $"MFA 启用失败: {ex.Message}");
            }

        }
        /// <summary>
        /// 获取用户基本信息
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        public async Task<Response<User_res>> GetUserAsync(common_req<User_req> signup_req)
        {
            User_res user_Res = new User_res();
            try
            {
                var email = signup_req.Actioninfo.Email + "";
                var password = signup_req.Actioninfo.Password + "";
                string pwd = "";
                //if (!string.IsNullOrEmpty(password))
                //{
                //    sqlwhere += " AND GooglelocalId=@GooglelocalId ";
                //}
                //获取登录账户基本信息
                string cmdText = @"select uId,PassWord,Nickname,Gender,Avatar,Birthdate,Mobilephone,CountryCode,AdminArea1,AdminArea2,AddressLine1,AddressLine2,GooglelocalId,EmailVerification,PhoneVerification from 
                    Users WHERE Status != '2'  AND email = @email ";
                SqlCommand cmd = new SqlCommand(cmdText);
                cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = email;
                cmd.CommandText = cmdText;
                DataTable table = new DataTable();
                GoogleSqlDBHelper.Fill(cmd, table);//获取用户是否存在
                if(table != null && table.Rows.Count > 0)
                {
                    bool isokpwd = Pub.VerifyPassword(password, (table.Rows[0]["PassWord"] + "").Trim());
                    if (!isokpwd)
                    {
                        return new Response<User_res>($"密码错误,请重新输入正确的账户密码！");
                    }
                    user_Res.Id = table.Rows[0]["uId"] + "";
                    user_Res.Nickname = table.Rows[0]["Nickname"] +"";
                    user_Res.Gender = table.Rows[0]["Gender"] + "";
                    user_Res.Avatar = table.Rows[0]["Avatar"] + "";
                    user_Res.Birthdate = table.Rows[0]["Birthdate"] + "";
                    user_Res.Mobilephone = table.Rows[0]["Mobilephone"] + "";
                    user_Res.CountryCode = table.Rows[0]["CountryCode"] + "";
                    user_Res.AdminArea1 = table.Rows[0]["AdminArea1"] + "";
                    user_Res.AdminArea2 = table.Rows[0]["AdminArea2"] + "";
                    user_Res.AddressLine1 = table.Rows[0]["AddressLine1"] + "";
                    user_Res.AddressLine2 = table.Rows[0]["AddressLine2"] + "";
                    user_Res.GooglelocalId = table.Rows[0]["GooglelocalId"] + "";
                    user_Res.EmailVerification = table.Rows[0]["EmailVerification"] + "";
                    user_Res.PhoneVerification = table.Rows[0]["PhoneVerification"] + "";
                    user_Res.UserRole = "0";
                    user_Res.UserPageList = "";
                    //是否员工登录
                   cmdText = @" SELECT top 1 sr.Id,sr.RoleType,ViewFile FROM Users u left join User_Identity sr on u.RId = sr.Id WHERE u.UId = '" + user_Res.Id + "' ";
                    table = new DataTable();
                    GoogleSqlDBHelper.Fill(cmdText,table);
                    if(table != null && table.Rows.Count > 0)
                    {
                        user_Res.UserRole = table.Rows[0]["RoleType"] + "";
                        user_Res.UserPageList = table.Rows[0]["ViewFile"] + "";
                    }
                    return new Response<User_res>(user_Res, "");
                }
                return new Response<User_res>($"获取账户信息失败: 未找到用户");
            }
            catch (Exception ex)
            {
                Pub.SaveLog(nameof(AccountService), $"获取账户信息失败: {ex.Message}");
                return new Response<User_res>($"获取账户信息失败: {ex.Message}");
            }
        }
        /// <summary>
        /// 获取用户个人信息
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response<UserInformation_res>> GetUserInformationAsync(common_req<string> signup_req)
        {
            string uid = signup_req.User;
            string cmdText = "";
            UserInformation_res userInformation_Res = new UserInformation_res();
            userInformation_Res.UserPersonInformation = new UserPersonInformationResponse();
            userInformation_Res.UserCompanyInformation = new UserCompanyInformationResponse();
            cmdText = @" select nickname as name,BirthDate as birthday,Gender as sex,email,Mobilephone as phone,Address,AdminArea1 as State,AdminArea2 as city,AddressLine1 as Street,Addressline2 as Apartment,PostalCode as zipcode,Socialsecuritynumber,domain from users where uid = '" + uid + "' ";
            DataTable table = new DataTable();
            GoogleSqlDBHelper.Fill(cmdText, table);
            if (table != null && table.Rows.Count > 0)
            {
                List<UserPersonInformationResponse> userPersonInformationResponse = Pub.ToList<UserPersonInformationResponse>(table);
                userInformation_Res.UserPersonInformation = userPersonInformationResponse.First();
                if (!string.IsNullOrEmpty(userInformation_Res.UserPersonInformation.Domain))
                {
                    cmdText = @" select name,Address,Phone,MainBusiness,Email,Zipcode,CompantNumber,TimeSheetType,BusinessType,Edd,EmployerIdentificationNumber as Ein,BankAccount,PayPeriod from Compant where id = '"+ userInformation_Res.UserPersonInformation.Domain + "' ";
                     table = new DataTable();
                    GoogleSqlDBHelper.Fill(cmdText, table);
                    if (table != null && table.Rows.Count > 0)
                    {
                        List<UserCompanyInformationResponse> userCompanyInformationResponses = Pub.ToList<UserCompanyInformationResponse>(table);
                        userInformation_Res.UserCompanyInformation = userCompanyInformationResponses.First();
                    }
                    }
                return new Response<UserInformation_res>(userInformation_Res,"");
            }
            return new Response<UserInformation_res>("");
        }

        /// <summary>
        /// 更新用户基本信息
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        public async Task<Response<string>> UpdateUserInformationAsync(common_req<string> signup_req)
        {
            string type = signup_req.Type;
            string user = signup_req.User;
            string changevalue = signup_req.Actioninfo;
            string setsql = "";
            int num = 0;
           List<string> parameterName = new List<string>();
            List<SqlDbType> parameterType = new List<SqlDbType>();
            List<string> parameterValue = new List<string>(); 
            //object parameterValue = changevalue; // 默认值为字符串

            switch (type.ToLower())
            {
                case "name":
                    setsql = "UPDATE users SET NickName = @NickName";
                    parameterName.Add("@NickName");
                    parameterType.Add(SqlDbType.NVarChar);
                    parameterValue.Add(changevalue);
                    break;
                case "birthday":
                    setsql = "UPDATE users SET BirthDate = @BirthDate";
                    parameterName.Add("@BirthDate");
                    parameterType.Add(SqlDbType.DateTime);
                    if (!DateTime.TryParse(changevalue, out DateTime birthDate))
                    {
                        throw new ArgumentException("BirthDate 格式无效。");
                    }
                    parameterValue.Add(birthDate.ToString());
                    break;
                case "gender":
                    setsql = "UPDATE users SET Gender = @Gender";
                    parameterName.Add("@Gender");
                    parameterType.Add(SqlDbType.Int);
                    if (!int.TryParse(changevalue, out int gender))
                    {
                        throw new ArgumentException("Gender 出现错误。");
                    }
                    parameterValue.Add(gender.ToString());
                    break;
                case "email":
                    setsql = "UPDATE users SET Email = @Email";
                    parameterName.Add("@Email");
                    parameterType.Add(SqlDbType.NVarChar);
                    parameterValue.Add(changevalue);
                    break;
                case "mobile": 
                    setsql = "UPDATE users SET Mobilephone = @Mobilephone";
                    parameterName.Add("@Mobilephone");
                    parameterType.Add(SqlDbType.NVarChar);
                    parameterValue.Add(changevalue);
                    break;
                case "address":
                    string [] addressarr = changevalue.Split(',');
                    string address = "";
                    setsql = "UPDATE users SET AdminArea1 = @AdminArea1,AdminArea2 = @AdminArea2,AddressLine1 = @AddressLine1,Addressline2 = @Addressline2,PostalCode = @PostalCode,Address = @Address ";
                    for (int i = 0;i< addressarr.Length; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                parameterName.Add("@AdminArea1");
                                parameterType.Add(SqlDbType.NVarChar);
                                address = addressarr[i];
                                break;
                            case 1:
                                parameterName.Add("@AdminArea2");
                                parameterType.Add(SqlDbType.NVarChar);
                                address = address +","+ addressarr[i];
                                break;
                            case 2:
                                parameterName.Add("@AddressLine1");
                                parameterType.Add(SqlDbType.NVarChar);
                                address = address + "," + addressarr[i];
                                break;
                            case 3:
                                parameterName.Add("@Addressline2");
                                parameterType.Add(SqlDbType.NVarChar);
                                address = address +","+ addressarr[i];
                                break;
                            case 4:
                                parameterName.Add("@PostalCode");
                                parameterType.Add(SqlDbType.VarChar);
                                break;
                        }
                        parameterValue.Add(addressarr[i]);
                    }
                    parameterName.Add("@Address");
                    parameterType.Add(SqlDbType.NVarChar);
                    parameterValue.Add(changevalue);
                    break;
                case "socialsecuritynumber":
                    setsql = "UPDATE users SET Socialsecuritynumber = @Socialsecuritynumber";
                    parameterName.Add("@Socialsecuritynumber");
                    parameterType.Add(SqlDbType.NVarChar);
                    parameterValue.Add(changevalue);
                    break;
                default:
                    throw new ArgumentException("无效的更新类型。");
            }

            if (!string.IsNullOrEmpty(setsql))
            {
                string cmdText = $"{setsql} WHERE uid = @uid";
                SqlCommand cmd = new SqlCommand(cmdText);
                cmd.Parameters.Add("@uid", SqlDbType.NVarChar).Value = user;
                for (int i = 0; i < parameterName.Count; i++) {
                    cmd.Parameters.Add(parameterName[i], parameterType[i]).Value = parameterValue[i];
                }
                 num = GoogleSqlDBHelper.ExecuteNonQuery(cmd);
            }
            if (num <= 0)
            {
                return new Response<string>("修改个人信息失败", "");
            }
            return new Response<string>("", "");
        }


    }

}
