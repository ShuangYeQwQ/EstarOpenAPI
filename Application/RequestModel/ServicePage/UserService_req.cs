using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.RequestModel.ServicePage
{
    /// <summary>
    /// 用户服务
    /// </summary>
    public class UserService_req
    {
        /// <summary>
        /// 权限
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// 输入查询的值
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 服务状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 页码数
        /// </summary>
        public int Page { get; set; }
    }
}
