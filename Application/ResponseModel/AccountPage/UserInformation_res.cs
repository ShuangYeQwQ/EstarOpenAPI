using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseModel.AccountPage
{
    public class UserInformation_res
    {
        /// <summary>
        /// 
        /// </summary>
        public UserPersonInformationResponse UserPersonInformation { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public UserCompanyInformationResponse UserCompanyInformation { get; set; }

        public class UserPersonInformationResponse
        {
            //公司id
            public string Domain { get; set; }
            //姓名
            public string Name { get; set; }
            //生日
            public string BirthDay { get; set; }
            //性别
            public string Sex { get; set; }
            //邮箱
            public string Email { get; set; }
            // 手机
            public string Phone { get; set; }
            // 地址
            public string Address { get; set; }
            // 邮编
            public string Zipcode { get; set; }
            // 社安号
            public string SocialSecurityNumber { get; set; }
        }
        public class UserCompanyInformationResponse
        {
            //公司名称
             public string Name { get; set; }
            //公司地址
             public string Address { get; set; }
            //公司电话
             public string Phone { get; set; }
            //主营业务
             public string MainBusiness { get; set; }
            //公司邮箱
             public string Email { get; set; }
            //公司邮编
             public string Zipcode { get; set; }
            //联邦报税号
             public string CompantNumber { get; set; }
            //薪酬计算类型
             public string TimeSheetType { get; set; }
            //公司形式
             public string BusinessType { get; set; }
            //edd
             public string Edd { get; set; }
            //ein
             public string Ein { get; set; }
            //银行账户
             public string BankAccount { get; set; }
            //发薪周期
             public string PayPeriod { get; set; }
        }

    }
}
