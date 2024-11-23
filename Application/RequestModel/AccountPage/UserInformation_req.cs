using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.RequestModel.AccountPage
{
    public class UserInformation_req
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public string Birth { get; set; }
        /// <summary>
        /// 社安号码
        /// </summary>
        public string Socialsecuritynumber { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 街道
        /// </summary>
        public string Streetaddress { get; set; }
        /// <summary>
        /// 公寓号
        /// </summary>
        public string Apartmentnumber { get; set; }
    }
}
