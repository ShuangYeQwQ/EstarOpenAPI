using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity.Models
{
    public class W2Model
    {
        /// <summary>
        /// 主键标识符
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// 用户服务详情ID
        /// </summary>
        public string UserServiceDetailId { get; set; }

        /// <summary>
        /// 记录创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 税务表格所属年份
        /// </summary>
        public string FormYear { get; set; }

        // 雇主信息
        public string EmployerName { get; set; }
        public string EmployerAddress_StreetAddressOrPostalBox { get; set; }
        public string EmployerAddress_City { get; set; }
        public string EmployerAddress_State { get; set; }
        public string EmployerAddress_Zip { get; set; }
        public string EIN { get; set; }  // 雇主识别号

        // 雇员信息
        public string EmployeeName_FirstName { get; set; }
        public string EmployeeName_LastName { get; set; }
        public string Suff { get; set; }  // 姓名后缀 (Jr., Sr.等)
        public string EmployeeAddress_StreetAddressOrPostalBox { get; set; }
        public string EmployeeAddress_City { get; set; }
        public string EmployeeAddress_State { get; set; }
        public string EmployeeAddress_Zip { get; set; }
        public string SSN { get; set; }  // 社会安全号码

        // 税务控制信息
        public string ControlNumber { get; set; }

        // 收入与税务扣缴
        public string WagesTipsOtherCompensation { get; set; }
        public string FederalIncomeTaxWithheld { get; set; }
        public string SocialSecurityWages { get; set; }
        public string SocialSecurityTaxWithheld { get; set; }
        public string MedicareWagesAndTips { get; set; }
        public string MedicareTaxWithheld { get; set; }
        public string SocialSecurityTips { get; set; }
        public string AllocatedTips { get; set; }
        public string DependentCareBenefits { get; set; }
        public string NonqualifiedPlans { get; set; }

        // 特殊代码区域
        public string Code12 { get; set; }
        public string Code12Value { get; set; }

        // 复选框标识
        public string StatutoryEmployee { get; set; }
        public string RetirementPlan { get; set; }
        public string ThirdPartySickPay { get; set; }
        public string Other { get; set; }

        // 州税务信息
        public string State_Line1 { get; set; }
        public string EmployerStateIdNumber_Line1 { get; set; }
        public string StateWagesTipsEtc_Line1 { get; set; }
        public string StateIncomeTax_Line1 { get; set; }

        // 地方税务信息
        public string LocalWagesTipsEtc { get; set; }
        public string LocalIncomeTax { get; set; }
        public string LocalityName { get; set; }
    }
}
