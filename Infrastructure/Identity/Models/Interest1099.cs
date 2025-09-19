using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity.Models
{
    public class Interest1099
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

        // 利息收入信息
        public decimal? InterestIncome { get; set; }
        public decimal? EarlyWithdrawalPenalty { get; set; }
        public decimal? InterestOnUSBonds { get; set; }
        public decimal? FederalIncomeTaxWithheld { get; set; }
        public decimal? InvestmentExpenses { get; set; }
        public decimal? ForeignTaxPaid { get; set; }
        public string ForeignCountryOrUSTerritory { get; set; }
        public decimal? TaxExemptInterest { get; set; }
        public decimal? SpecifiedPrivateActivityBondInterest { get; set; }
        public decimal? MarketDiscount { get; set; }
        public decimal? BondPremium { get; set; }
        public decimal? BondPremiumOnTreasuryObligations { get; set; }
        public decimal? BondPremiumOnTaxExemptBond { get; set; }
        public string TaxExemptBondCUSIP { get; set; }

        // 州税务信息
        public string State { get; set; }
        public string StateIdentificationNo { get; set; }
        public decimal? StateTaxWithheld { get; set; }

        // 支付方信息
        public string PayerName { get; set; }
        public string PayerStreetAddress { get; set; }
        public string PayerCity { get; set; }
        public string PayerState { get; set; }
        public string PayerZIP { get; set; }
        public string PayerTelephone { get; set; }
        public string PayerTIN { get; set; }

        // 收款方信息
        public string RecipientTIN { get; set; }
        public string RecipientName { get; set; }
        public string RecipientStreetAddress { get; set; }
        public string RecipientCity { get; set; }
        public string RecipientState { get; set; }
        public string RecipientZIP { get; set; }

        // 其他信息
        public string AccountNumber { get; set; }
        public bool? FATCAFilingRequirement { get; set; }
    }
}
