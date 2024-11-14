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

namespace Infrastructure.Shared
{
    public class Pub
    {
        public ILogger<HomePageService> _logger { get; }
        public Pub(ILogger<HomePageService> logger)
        {
            _logger = logger;
        }
        private const int SaltSize = 16; // 盐的大小
        private const int HashSize = 32; // 哈希的大小
        private const int Iterations = 10000; // 迭代次数
        private static long lLeft = 621355968000000000;
        private readonly static string appSecret =  ConfigurationManager.AppSettings["appSecret"] + "";
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

        //地球半径，单位米
        private const double EARTH_RADIUS = 6378.137;
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
        public static void SendEmail(string recipientEmail, string subject, string body,ref string msg)
        {
            string _smtpServer = "smtp.gmail.com"; // SMTP 服务器地址
            int _smtpPort = 587; // 端口（587 用于 TLS）
            string _username = "yangyiz2024@gmail.com"; // 发件人邮箱
            string _password = "e v k g y w f l k n z e z y w w"; // 发件人邮箱密码
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("YourAppName", _username));
            message.To.Add(new MailboxAddress(recipientEmail, recipientEmail));
            message.Subject = subject;
            // 使用 BodyBuilder 创建 HTML 内容
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;

            message.Body = bodyBuilder.ToMessageBody();
            SmtpClient client = null;
            try
            {
                client = new SmtpClient();
                // 连接到 SMTP 服务器
                client.Connect(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                // 身份验证
                client.Authenticate(_username, _password);
                // 发送邮件
                client.Send(message);
            }
            catch (Exception ex)
            {
                msg = $"Email sending failed: {ex.Message}";
                // 记录错误或采取适当的行动
            }
            finally
            {
                // 确保客户端断开连接
                if (client != null && client.IsConnected)
                {
                    client.Disconnect(true);
                }
                client?.Dispose(); // 释放资源
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
        /// <param name="password"></param>
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
    }
}
