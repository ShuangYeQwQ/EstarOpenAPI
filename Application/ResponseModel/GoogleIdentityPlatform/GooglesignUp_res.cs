using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseModel.GoogleIdentityPlatform
{
    public class GooglesignUp_res
    {
        /// <summary>
        /// 请求
        /// </summary>
        public string kind { get; set; }
        /// <summary>
        /// 已创建用户的 Identity Platform ID 令牌
        /// </summary>

        public string idToken { get; set; }
        /// <summary>
        /// 创建的用户的显示名称
        /// </summary>

        public string displayName { get; set; }
        /// <summary>
        /// 已创建用户的电子邮件
        /// </summary>

        public string email { get; set; }
        /// <summary>
        /// 已创建用户的 Identity Platform 刷新令牌
        /// </summary>

        public string refreshToken { get; set; }
        /// <summary>
        /// 令牌过期前的秒数
        /// </summary>

        public string expiresIn { get; set; }
        /// <summary>
        /// 已创建用户的 ID
        /// </summary>

        public string localId { get; set; }
    }
}
