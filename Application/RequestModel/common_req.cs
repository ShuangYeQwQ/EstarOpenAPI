using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.RequestModel
{
    public class common_req<T>
    {
        /// <summary>
        /// 请求用户
        /// </summary>
        public string user { get; set; }
        /// <summary>
        /// 请求用户的公司域
        /// </summary>
        public string domain { get; set; }
        /// <summary>
        /// 请求类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 请求数据对象
        /// </summary>
        public T actioninfo { get; set; }

    }
}
