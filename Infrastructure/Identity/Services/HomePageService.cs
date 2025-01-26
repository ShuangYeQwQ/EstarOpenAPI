using Application.Interfaces;
using Application.RequestModel.HomePage;
using Application.ResponseModel.HomePage;
using Application.Wrappers;
using Infrastructure.Identity.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using System.Text;
using Infrastructure.Shared;
using EStarGoogleCloud;
using System.Data;
using Application.RequestModel;
using GoogleCloudModel;
using Azure.Core;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Data.Common;
using static Google.Cloud.DocumentAI.V1.Document.Types.Page.Types;



namespace Infrastructure.Identity.Services
{
    public class HomePageService : IHomePageService
    {
        public ILogger<HomePageService> _logger { get; }
        public HomePageService(ILogger<HomePageService> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// 注册账号
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response<Signup_res>> RegisterAsync(common_req<Signup_req> signup_req)
        {
            try
            {
                //验证输入的验证码是否正确
                string type = signup_req.type + "";//0:邮箱验证，1:手机验证
                var action_info = signup_req.actioninfo;
                string username = action_info.nickname + "";
                string password = action_info.password + "";
                string email = action_info.email + "";
                string verificationcode = action_info.verificationcode + "";
                Signup_res signup_Res = new Signup_res();
                string sql = "";
                string code = "";
                Users user = new Users();
                string msg = "";
                var command = new SqlCommand();
                //command.Parameters.AddWithValue("@email", email);
                if (type.Equals("0"))
                {
                   
                    sql = string.Format($@" SELECT top 1 Code FROM otp_verify where email = @email  and ExpiryTime > GETDATE()  order by Sendtime DESC ");
                    command = new SqlCommand(sql);
                    command.Parameters.AddWithValue("@email", email);
                    code = GoogleSqlDBHelper.ExecuteScalar(command);
                    if (!code.Equals(verificationcode) || !string.IsNullOrEmpty(msg))
                    {
                        Pub.SaveLog(nameof(HomePageService), "验证码错误，请检查您输入的验证码是否与接收的一致");
                        return new Response<Signup_res>("验证码错误，请检查您输入的验证码是否与接收的一致");
                    }
                    user.Email = email;
                }
                else
                {
                    //sql = string.Format(@" SELECT top 1 Code FROM otp_verify where MobilePhone = @email   order by Sendtime DESC ");
                    //code = GoogleSqlDBHelper.ExecuteScalar(sql, command, ref msg);
                    //if (!code.Equals(verificationcode) || string.IsNullOrEmpty(msg))
                    //{
                    //    return new Response<Signup_res>("验证码错误，请检查您输入的验证码是否与接收的一致");
                    //}
                    //注册账号
                    user.Mobilephone = email;
                }
                user.password = Pub.HashPassword(password);
                user.Username = username;
                user.Nickname = username;
                user.Createdate = DateTime.Now;
                user.Updatedate = DateTime.Now;
                user.Status = 1;
                user.account = email;
                List<DbCommand> dbcom = new List<DbCommand>();
                command = new SqlCommand(string.Format(@" INSERT INTO Users(Account, Password, UserName, CreateDate, UpdateDate, Access_Token, Expires_in, Refiesh_token, NickName, Gender, Avatar, Birthdate, Address, Status, Email, Mobilephone, Domain) 
VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}','{14}', '{15}', '{16}')  ",
email, user.password, user.Username, user.Createdate, user.Updatedate,"",DateTime.Now,"", user.Username,"0","", DateTime.Now,"", user.Status, user.Email, user.Mobilephone,""));

                dbcom.Add(command);
                int num = GoogleSqlDBHelper.ExecuteNonQueryTransaction(dbcom, ref msg);
                if(num > 0)
                {
                    sql = string.Format(@" SELECT top 1 id FROM users where Account = @email ");
                    command = new SqlCommand(sql);
                    command.Parameters.AddWithValue("@email", email);
                    string account = GoogleSqlDBHelper.ExecuteScalar(command);
                    if (string.IsNullOrEmpty(account))
                    {
                        return new Response<Signup_res>("");
                    }
                    string token = Pub.GenerateToken(account);
                    string refieshtoken = Pub.GenerateRefreshToken(account);
                    sql = " update Users set  Access_Token = @Access_Token,Expires_in = @Expires_in, Refiesh_token = @Refiesh_token where id = @id ";

                    command = new SqlCommand();
                    command.Parameters.AddWithValue("@id", account);
                    command.Parameters.AddWithValue("@Access_Token", token);
                    command.Parameters.AddWithValue("@Expires_in", DateTime.Now.AddDays(30));
                    command.Parameters.AddWithValue("@Refiesh_token", refieshtoken);
                    num = GoogleSqlDBHelper.ExecuteNonQuery(command);
                    if(num > 0)
                    {
                        signup_Res.user = account;
                        signup_Res.username = user.Username;
                        signup_Res.token = token;
                        signup_Res.accessTokenExpiration = (((DateTimeOffset)DateTime.Now.AddHours(2)).ToUnixTimeSeconds()).ToString();
                        signup_Res.refiesh_token = refieshtoken;
                        return new Response<Signup_res>(signup_Res, "注册成功！");
                    }
                }
                string logSql = Pub.ReplaceSqlParameters(sql, command);
                Pub.SaveLog(nameof(HomePageService), $"注册失败,SQL:{logSql}");
                return new Response<Signup_res>("注册失败");
            }
            catch (Exception ex)
            {
                Pub.SaveLog(nameof(HomePageService), $"注册失败,错误信息：{ex.Message}");
                return new Response<Signup_res>(ex.Message);
            }
        }
        /// <summary>
        /// 邮箱验证
        /// </summary>
        /// <param name="user">登录用户</param>
        /// /// <param name="type">0:注册验证，1：登陆验证</param>
        /// <param name="email">验证码接收email或手机号</param>
        /// <returns></returns>
        public async Task<Response<string>> SendVerificationEmailAsync(common_req<Signup_req> signup_req)
        {
            string user = signup_req.user + "";
            string email = signup_req.actioninfo.email + "";
            string type = signup_req.type + "";//0:邮箱验证，1:手机验证
            string msg = "";
            var command = new SqlCommand();
            try
            {
                if (email != null && email != "" && type != null && type != "")
                {
                    //获取邮件信息模板配置
                     string sql = string.Format(" SELECT EmailBody,EmailSubject FROM common_config ");
                    DataTable dt = new DataTable();
                    GoogleSqlDBHelper.Fill(sql, dt);
                     if (dt != null && dt.Rows.Count > 0)
                    {
                    string code = Pub.GenerateOtp();
                    string subject = dt.Rows[0]["EmailSubject"] + "";
                    string body = dt.Rows[0]["EmailBody"] + "";
                    string newBody = body.Replace("{userEmail}", email);
                    newBody = newBody.Replace("{oneTimeCode}", code);
                    if (type.Equals("0"))
                    {
                        int num = Pub.SendEmail(email, subject, newBody);
                        if (num <= 0)
                        {
                            return new Response<string>("邮件发送失败!");
                        }
                            sql = string.Format($@"insert into otp_verify(code,Sendtime,ExpiryTime,Status,VerifyTime,MobilePhone,Type,Email,EmailBody,EmailSubject,retrycount) 
values(@code,@Sendtime,@ExpiryTime,@Status,@VerifyTime,@MobilePhone,@Type,@Email,@EmailBody,@EmailSubject,@retrycount)");
                            command = new SqlCommand();
                            command.Parameters.AddWithValue("@code", code);
                            command.Parameters.AddWithValue("@Sendtime", DateTime.Now);
                            command.Parameters.AddWithValue("@ExpiryTime", DateTime.Now.AddMinutes(5));
                            command.Parameters.AddWithValue("@Status", "0");
                            command.Parameters.AddWithValue("@VerifyTime", DateTime.Now);
                            command.Parameters.AddWithValue("@MobilePhone", "");
                            command.Parameters.AddWithValue("@Type", type);
                            command.Parameters.AddWithValue("@Email", email);
                            command.Parameters.AddWithValue("@EmailBody", newBody);
                            command.Parameters.AddWithValue("@EmailSubject", subject);
                            command.Parameters.AddWithValue("@retrycount", "0");
                            num = GoogleSqlDBHelper.ExecuteNonQuery(command);

                          if( num > 0)
                            {
                                return new Response<string>("ok", "邮件发送成功");
                            }
                    }
                    else if (type.Equals("1"))
                    {
                        //获取登录过期用户

                        return new Response<string>("", "");
                    }
                      }
                      else
                      {
                        Pub.SaveLog(nameof(HomePageService), $"出错,请在基础配置表(common_config)添加邮件主题，内容等配置！提示：{msg}");
                          return new Response<string>("验证失败！所需配置未完善");
                      }
                }
                Pub.SaveLog(nameof(HomePageService), $"验证失败,email:{email}" );
                return new Response<string>("验证失败!" + "email:" + email);
            }
            catch (Exception ex)
            {
                Pub.SaveLog(nameof(HomePageService), $"验证失败,email: {email}, 提示:{ex.Message}");
                return new Response<string>("验证失败!" + ex.Message);
            }


        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user">登录用户</param>
        /// /// <param name="type">0:注册验证，1：登陆验证</param>
        /// <param name="email">验证码接收email或手机号</param>
        /// <returns></returns>
        public async Task<Response<Signup_res>> LoginAsync(common_req<Signup_req> signup_req)
        {
            string user = signup_req.user + "";
            string email = signup_req.actioninfo.email + "";
            string password = signup_req.actioninfo.password + "";//0:邮箱验证，1:手机验证
            string msg = "";
            Signup_res signup_Res = new Signup_res();
            try
            {
                string epassword = "";
                bool isok = true;// Pub.VerifyPassword(password, epassword);
                if (isok)
                {
                signup_Res.token = "xx";
                signup_Res.refiesh_token = "xx";
                signup_Res.accessTokenExpiration = "xx";
                signup_Res.user = "xx";
                signup_Res.username = "xx";
                return new Response<Signup_res>(signup_Res, "登录成功！");
                }
                else
                {
                    return new Response<Signup_res>("登录失败，密码输入错误");
                }
                
            }
            catch (Exception ex)
            {
                Pub.SaveLog(nameof(HomePageService), $"登录失败!email：{email}，错误提示：{ex.Message}" );
                return new Response<Signup_res>("登录失败!" + ex.Message);
            }
        }
    }
}
