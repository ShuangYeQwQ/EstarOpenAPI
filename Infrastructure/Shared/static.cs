using System.Net.Sockets;
using System.Net;
using MailKit.Net.Smtp;
using MimeKit;
using Infrastructure.Identity.Services;
using Microsoft.Extensions.Logging;
using Google.LongRunning;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Configuration;
using GoogleCloudModel;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Data;
using System.Reflection;
using iText.Kernel.Pdf;
using iText.Forms;
using iText.Forms.Fields;
using static Google.Cloud.DocumentAI.V1.TrainProcessorVersionRequest.Types;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Infrastructure.Shared
{
    public static class Pub
    {
        //地球半径，单位米
        private const double EARTH_RADIUS = 6378.137;
        private const int SaltSize = 16; // 盐的大小
        private const int HashSize = 32; // 哈希的大小
        private const int Iterations = 10000; // 迭代次数
        private static long lLeft = 621355968000000000;
        private readonly static string appSecret =  ConfigurationManager.AppSettings["appSecret"] + "";
        private static readonly object _lock = new object();//日志锁定
        //将数字变成时间
        public static string GetTimeFromInt(long ltime)
        {

            long Eticks = (long)(ltime * 10000000 / 1000) + lLeft;
            DateTime dt = new DateTime(Eticks).ToLocalTime();
            return dt.ToString();

        }
        //将时间变成数字
        public static long GetIntFromTime(DateTime dt)
        {
            DateTime dt1 = dt.ToUniversalTime();
            long Sticks = (dt1.Ticks - lLeft) / 10000000;
            return Sticks * 1000;

        }

        
        /// <summary>
        /// 计算两点位置的距离，返回两点的距离，单位 米   通过经纬度计算2个点之间距离
        /// 该公式为GOOGLE提供，误差小于0.2米
        /// </summary>
        /// <param name="lat1">第一点纬度</param>
        /// <param name="lng1">第一点经度</param>
        /// <param name="lat2">第二点纬度</param>
        /// <param name="lng2">第二点经度</param>
        /// <returns></returns>
        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = Rad(lat1);
            double radLng1 = Rad(lng1);
            double radLat2 = Rad(lat2);
            double radLng2 = Rad(lng2);
            double a = radLat1 - radLat2;
            double b = radLng1 - radLng2;
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
                Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }

        /// <summary>
        /// 经纬度转化成弧度
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double Rad(double d)
        {
            return d * Math.PI / 180d;
        }
        /// <summary>
        /// 获取IPv4地址
        /// </summary>
        /// <returns></returns>
        public static string GetIpAddress(ref string msg)
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            catch (SocketException ex)
            {
                // Handle socket exceptions (e.g., host not found)
                msg = $"Socket exception: {ex.Message}";
            }
            catch (Exception ex)
            {
                // Handle other potential exceptions
                msg = $"An error occurred: {ex.Message}";
            }
            return string.Empty; // Return an empty string if no valid IP is found
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="recipientEmail">收件人</param>
        /// <param name="subject">发件主题</param>
        /// <param name="body">发送内容</param>
        public static int SendEmail(string recipientEmail, string subject, string body)
        {
            string smtpServer = "smtp.gmail.com"; // SMTP 服务器地址
            int smtpPort = 587; // 端口（587 用于 TLS）
            string username = "yangyiz2024@gmail.com"; // 发件人邮箱
            string password = "e v k g y w f l k n z e z y w w"; // 发件人邮箱密码
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("YourAppName", username));
            message.To.Add(new MailboxAddress(recipientEmail, recipientEmail));
            message.Subject = subject;
            // 使用 BodyBuilder 创建 HTML 内容
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;

            message.Body = bodyBuilder.ToMessageBody();
            using (var client = new SmtpClient())
            {
                try
                {
                    // 连接到 SMTP 服务器
                    client.Connect(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);

                    // 验证身份
                    client.Authenticate(username, password);

                    // 发送邮件
                    client.Send(message);

                    // 返回成功状态
                    return  1;
                }
                catch (Exception ex)
                {
                    SaveLog("SendEmailService", $"Email sending failed: {ex.Message}");
                }
                finally
                {
                    // 确保客户端断开连接
                    if (client.IsConnected)
                        client.Disconnect(true);
                }
                return 0;
            }
        }
        /// <summary>
        /// 生成6位随机数
        /// </summary>
        /// <returns></returns>
        public static string GenerateOtp()
        {
            Random random = new Random();
            string otp = random.Next(100000, 999999).ToString();
            return otp;
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="password">需要加密的密码</param>
        /// <returns></returns>
        public static string HashPassword(string password)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                // 生成盐
                byte[] salt = new byte[SaltSize];
                rng.GetBytes(salt);

                // 生成哈希
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
                {
                    byte[] hash = pbkdf2.GetBytes(HashSize);

                    // 将盐和哈希组合在一起，方便存储
                    byte[] hashBytes = new byte[SaltSize + HashSize];
                    Array.Copy(salt, 0, hashBytes, 0, SaltSize);
                    Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

                    return Convert.ToBase64String(hashBytes);
                }
            }
        }
        /// <summary>
        /// 密码验证
        /// </summary>
        /// <param name="password">验证密码</param>
        /// <param name="storedHash">存储密码</param>
        /// <returns></returns>
        public static bool VerifyPassword(string password, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // 提取盐
            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // 计算哈希
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);

                // 将计算得到的哈希与存储的哈希进行比较
                for (int i = 0; i < HashSize; i++)
                {
                    if (hashBytes[i + SaltSize] != hash[i])
                    {
                        return false; // 密码不匹配
                    }
                }
            }
            return true; // 密码匹配
        }
        /// <summary>
        /// 生成secretKey
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string GenerateRandomKey(int size = 32)
        {
            var key = new byte[size];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(key);
            }
            return Convert.ToBase64String(key);
        }
        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GenerateToken(string userId)
        {
           string s = ConfigurationManager.AppSettings["appSecret"] + "";
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId), //用户id
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),//
        //new Claim(JwtRegisteredClaimNames.Iss, "https://estar.com"), // 设置发行者
        //new Claim(JwtRegisteredClaimNames.Aud, "https://estar.com/api") // 设置受众
            new Claim(ClaimTypes.Role, ""), //用户访问权限id
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("bff3f90f3989ee330999fabafdb36156"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                  //issuer: "https://estar.com",
        //audience: "https://estar.com/api",
                expires: DateTime.Now.Add(TimeSpan.FromMinutes(120)),
                claims: claims,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        /// <summary>
        /// 生成RefreshToken
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GenerateRefreshToken(string userId)
        {
            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.NameIdentifier, userId), //用户id
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),//
        //new Claim(JwtRegisteredClaimNames.Iss, "https://estar.com"), // 设置发行者
        //new Claim(JwtRegisteredClaimNames.Aud, "https://estar.com/api") // 设置受众
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("bff3f90f3989ee330999fabafdb36156"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var refreshtoken = new JwtSecurityToken(
                //issuer: "https://estar.com",
                //audience: "https://estar.com/api",
                expires: DateTime.Now.Add(TimeSpan.FromDays(30)),
                claims: claims,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(refreshtoken);
        }
        /// <summary>
        /// 验证token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static ClaimsPrincipal ValidateToken(string token)
        {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(appSecret));
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // 验证令牌的有效性
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = false, // 可根据需要设置为 true
                    ValidateAudience = false, // 可根据需要设置为 true
                    ClockSkew = TimeSpan.Zero // 防止延迟导致的过期
                }, out SecurityToken validatedToken);

                return principal; // 返回验证后的 ClaimsPrincipal
            }
            catch (SecurityTokenExpiredException)
            {
                Console.WriteLine("令牌已过期.");
                return null; // 令牌过期
            }
            catch (SecurityTokenException ex)
            {
                Console.WriteLine($"令牌验证失败: {ex.Message}");
                return null; // 令牌无效
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}");
                return null; // 其他错误
            }
        }

        
        /// <summary>
        /// 存储日志
        /// </summary>
        /// <param name="serviceName">使用的服务名称</param>
        /// <param name="errorMessage">错误信息</param>
        public static void SaveLog(string serviceName, string errorMessage)
        {
            try
            {
                // 日志文件夹路径 (相对于当前项目路径)
                string logFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");

                // 如果日志文件夹不存在，则创建
                if (!Directory.Exists(logFolderPath))
                {
                    Directory.CreateDirectory(logFolderPath);
                }

                // 生成日志文件名：2024-11-15.log
                string logFileName = $"{DateTime.Now:yyyy-MM-dd}.log";
                string logFilePath = Path.Combine(logFolderPath, logFileName);

                // 当前时间，格式：2024-11-15 11:42:01.310
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

                // 日志内容格式：2024-11-15 11:42:01.310_ServiceName: 错误信息
                string logContent = $"{timestamp}_{serviceName}: {errorMessage}";

                // 使用锁定机制确保多线程环境下安全写入日志
                lock (_lock)
                {
                    // 将日志写入文件，追加模式
                    using (StreamWriter writer = new StreamWriter(logFilePath, true))
                    {
                        writer.WriteLine(logContent);
                        writer.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write log: {ex.Message}");
            }
        }
        /// <summary>
        /// 替换参数化sql的值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static string ReplaceSqlParameters(string sql, SqlCommand cmd)
        {
            try
            {
                foreach (SqlParameter param in cmd.Parameters)
                {
                    // 替换 SQL 中的参数占位符 @ParamName 为实际值
                    string paramValue = param.Value != DBNull.Value ? param.Value.ToString() : "NULL";
                    sql = sql.Replace(param.ParameterName, paramValue);
                }
            }
            catch (Exception ex)
            {
                Pub.SaveLog("PubService", $"替换参数化sql的值时发生异常:{ex.Message} ,SQL:{sql},CMD:{cmd.Parameters}");
            }
            return sql;
        }
        /// <summary>
        /// 生成随机密码
        /// </summary>
        /// <param name="length">密码位数</param>
        /// <returns></returns>
        public static string GeneratePassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; // 字母和数字
            Random random = new Random();
            char[] password = new char[length];

            for (int i = 0; i < length; i++)
            {
                password[i] = validChars[random.Next(validChars.Length)];
            }

            return new string(password);
        }

        /// <summary>
        /// 通用类型转换方法
        /// </summary>
        public static T To<T>(string input, T defaultValue = default)
        {
            try
            {
                return (T)Convert.ChangeType(input, typeof(T), CultureInfo.InvariantCulture);
            }
            catch
            {
                return defaultValue;
            }
        }
        /// <summary>
        /// 将 DataTable 转换为指定类型的实体列表
        /// </summary>
        /// <typeparam name="T">目标实体类型</typeparam>
        /// <param name="table">数据表</param>
        /// <returns>实体列表</returns>
        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            // 如果表为空，返回空列表
            if (table == null || table.Rows.Count == 0)
            {
                return new List<T>();
            }

            // 获取目标类型的所有属性
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // 创建列表
            var list = new List<T>();

            foreach (DataRow row in table.Rows)
            {
                var item = new T();
                foreach (var property in properties)
                {
                    // 检查列名是否存在且值不为 DBNull
                    if (table.Columns.Contains(property.Name) && row[property.Name] != DBNull.Value)
                    {
                        // 设置属性值
                        property.SetValue(item, Convert.ChangeType(row[property.Name], property.PropertyType));
                    }
                }
                list.Add(item);
            }

            return list;
        }
        public static List<T> ToList<T>(IEnumerable<T> query)
        {
            return query.ToList();
        }
        /// <summary>
        /// 数据转为pdf表单模板
        /// </summary>
        /// <param name="servicename">服务名称</param>
        /// <param name="templatePath">模板文件地址</param>
        /// <param name="pathname">转换后pdf文件</param>
        /// <param name="formData">数据</param>
        public static int FillPdfFormAndSaveNew(string servicename,string templatePath, string pathname,Dictionary<string, string> formData)
        {
            //string templatePath = @"C:\work\EStarOpenAPI\EstarOpenAPI\File\W-2.pdf";
            //string outputPath = @"C:\work\EStarOpenAPI\EstarOpenAPI\File\New_W-2_filled.pdf";
            int num = 1;
            try
            {
                // 打开模板PDF
                //using (PdfReader reader = new PdfReader(templatePath))
                //{
                //    // 创建一个新的文件输出流
                //    using (FileStream fs = new FileStream(pathname, FileMode.Create))
                //    {
                //        // 创建 PdfStamper 来编辑 PDF
                //        using (PdfStamper stamper = new PdfStamper(reader, fs))
                //        {
                //            // 获取PDF中的表单
                //            AcroFields fields = stamper.AcroFields;
                //            foreach (var field in formData)
                //            {
                //                var formField = field.Key;
                //                var formFievalue = field.Value;
                //                if (!string.IsNullOrEmpty(formField) && !string.IsNullOrEmpty(formFievalue))
                //                {
                //                    fields.SetField(formField, field.Value);
                //                }
                //            }
                //            // 设置输出PDF的表单不可编辑
                //            stamper.FormFlattening = false;

                //            // 保存修改后的PDF
                //            stamper.Close();
                //        }
                //    }
                //}
                // 使用 PdfReader 读取 PDF 模板
                using (PdfReader reader = new PdfReader(templatePath))
                using (PdfWriter writer = new PdfWriter(pathname))
                using (PdfDocument pdfDoc = new PdfDocument(reader, writer))
                {
                    // 获取 PDF 表单
                    PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);

                    if (form != null)
                    {
                        // 填充字段
                        foreach (var field in formData)
                        {
                            string fieldName = field.Key;
                            string fieldValue = field.Value;

                            // 检查字段是否存在并填充
                            if (!string.IsNullOrEmpty(fieldName) && !string.IsNullOrEmpty(fieldValue))
                            {
                                form.GetField(fieldName).SetValue(fieldValue);
                            }
                        }

                        // 设置表单为不可编辑
                        form.FlattenFields();
                    }
                    // 保存修改后的 PDF
                    pdfDoc.Close();
                }
                return num;
            }
            catch (Exception ex)
            {
                SaveLog(nameof(servicename), $"生成OCR识别后pdf模板失败：{ex.Message}");
                num = 0;
                return num;
            }
            

            //Console.WriteLine("PDF form has been filled and saved successfully.");
        }
        /// <summary>
        /// 获取pdf表单值
        /// </summary>
        /// <param name="servicename"></param>
        /// <param name="templatePath"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetFilePdfForm(string servicename, string templatePath)
        {
            Dictionary<string, string> formData = new Dictionary<string, string>();
            try
            {
                //using (PdfReader reader = new PdfReader(templatePath))
                //{
                //    // 获取 PDF 中的表单
                //    AcroFields fields = reader.AcroFields;

                //    // 获取并打印表单字段的名称和内容
                //    foreach (var field in fields.Fields)
                //    {
                //        string key = field.Key;
                //        string value = fields.GetField(field.Key);
                //        if (!string.IsNullOrEmpty(key))
                //        {
                //            formData.Add(key, value);
                //        }
                //        //Console.WriteLine($"Field Name: {field.Key}, Field Value: {fields.GetField(field.Key)}");
                //    }
                //}
                // 打开PDF文档
                using (PdfReader reader = new PdfReader(templatePath))
                using (PdfDocument pdf = new PdfDocument(reader))
                {
                    PdfAcroForm form = PdfAcroForm.GetAcroForm(pdf, false);

                    if (form != null)
                    {
                        var fields = form.GetAllFormFields();

                        foreach (var field in fields)
                        {
                            string key = field.Key;
                            string value = field.Value.GetValueAsString();
                            formData.Add(key, value);
                            //string fieldName = field.Key;
                            //string fieldValue = field.Value.GetValueAsString();

                            //Console.WriteLine($"字段名称：{fieldName}, 值：{fieldValue}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("未找到任何表单字段！");
                    }
                }
                return formData;
            }
            catch (Exception ex)
            {
                SaveLog(nameof(servicename), $"生成OCR识别后pdf模板失败：{ex.Message}");
                return formData;
            }
            
        }
    }
}
