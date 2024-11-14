using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity.Models
{
    public class OrderModel
    {
        public OrderModel()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; }
        /// <summary>
        /// 下单用户编号
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// 下单项目编号
        /// </summary>
        public string Projectid { get; set; }
    }
}
