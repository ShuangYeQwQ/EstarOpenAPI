using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseModel.ServicePage
{
    public class ServiceItem_res
    {
        /// <summary>
        /// 变量id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 变量名称
        /// </summary>
        public string ItemsName { get; set; }
        /// <summary>
        /// 是否必填0:否1:是
        /// </summary>
        public string IsRequired { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Descs { get; set; }
        /// <summary>
        /// 可输入变量类型
        /// </summary>
        public string ItemsType { get; set; }
        /// <summary>
        /// 输入变量最小值
        /// </summary>
        public string ItemMinInterval { get; set; }
        /// <summary>
        /// 输入变量最大值
        /// </summary>
        public string ItemMaxInterval { get; set; }
        /// <summary>
        /// 服务变量下不同值对应金额集合
        /// </summary>
        public List<ServiceItemValueList_res> serviceitem { get; set; }
    }
    public class ServiceItemValueList_res
    {
        /// <summary>
        /// 变量值id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 变量id
        /// </summary>
        public string ItemsId { get; set; }
        /// <summary>
        /// 区间最小数
        /// </summary>
        public string ItemsMinNumber { get; set; }
        /// <summary>
        /// 区间最大数
        /// </summary>
        public string ItemsMaxNumber { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public string BaseValue { get; set; }
        /// <summary>
        /// 每多x
        /// </summary>
        public string ItemsMax { get; set; }
        /// <summary>
        /// 每多x增加金额
        /// </summary>
        public string AdditionalValue { get; set; }
    }
}
