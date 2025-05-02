using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.RequestModel.PayPage
{

    /// <summary>
    /// paypal支付链接模板请求
    /// </summary>
    public class PayPal_req
    {
        //购买的服务id
        public string ServiceId { get; set; }
        /// <summary>
        /// 是否需要付费0：需要，1：不需要
        /// </summary>
        public string PayType { get; set; }
        /// <summary>
        /// 支付明细
        /// </summary>
        public List<PayItem> PayItems { get; set; }
        //需支付的总额
        public string TotalMoney { get; set; }
        //备注
        public string PayDesc { get; set; }
    }
    /// <summary>
    /// paypal支付
    /// </summary>
    public class PayPayPayment_req
    {
        /// <summary>
        /// 支付订单号
        /// </summary>
        public string Oid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PayerId { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
       // public string BeginServiceDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        //public string EndServiceDate { get; set; }
        /// <summary>
        /// 支付明细
        /// </summary>
        public List<PayItem> PayItems { get; set; }
    }
    /// <summary>
    /// 支付列表
    /// </summary>
    public class PayItem
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public string Num { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public string Money { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 服务类型  0:基础服务   1: 服务变量  2:附加服务
        /// </summary>
        public string Type { get; set; }

    }

}
