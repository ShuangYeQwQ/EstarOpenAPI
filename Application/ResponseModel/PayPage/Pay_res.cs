using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseModel.PayPage
{
    public class Pay_res
    {
        /// <summary>
        /// stripe支付链接
        /// </summary>
        public string clientsecret { get; set; }
        /// <summary>
        /// stripe支付id
        /// </summary>

        public string paymentintentid { get; set; }
    }
}
