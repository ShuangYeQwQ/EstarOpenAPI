using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Request.CchWorkSheet
{
    public class Dividends1099Form
    {
        // 税务报告类型（枚举类型）
        public string TSJ { get; set; }

        // 报表类型（枚举类型）
        public string FS { get; set; }

        // 州代码
        public string StateCode { get; set; }

        // 城市名称
        public string City { get; set; }

        // 普通股息（前年度）
        public string OrdinaryDividendsPY { get; set; }

        // 支付方名称
        public string PayerName { get; set; }

        // 支付方地址
        public string PayerAddress { get; set; }

        // 普通股息
        public float? OrdinaryDividends { get; set; }

        // 支付方城市
        public string PayerCity { get; set; }

        // 支付方州
        public string PayerState { get; set; }

        // 支付方邮政编码
        public string PayerZIP { get; set; }

        // 支付方电话号码
        public string PayerTelephone { get; set; }

        // 支付方税务识别号码（TIN）
        public string PayerTIN { get; set; }

        // 支付方州ID号
        public string PayerStateID { get; set; }

        // 合格股息
        public float? QualifiedDividends { get; set; }

        // 外国/省份
        public string ForeignCountryProvince { get; set; }

        // 外国邮政编码
        public string ForeignPostalCode { get; set; }

        // 外国国家代码
        public string ForeignCountryCode { get; set; }

        // 总资本增益分配
        public float? TotalCapitalGainDistr { get; set; }

        // 未回收的第1250条款增益
        public float? UnrecapturedSec1250Gain { get; set; }

        // 第1202条款增益
        public float? Section1202Gain { get; set; }

        // 28%税率增益
        public float? Rate28Gain { get; set; }

        // 支付方的税务识别号码
        public string PayerID { get; set; }

        // 收款方的税务识别号码（TIN）
        public string RecipientTIN { get; set; }

        // 第897条款普通股息
        public float? Section897OrdinaryDividends { get; set; }

        // 第897条款资本增益
        public float? Section897CapitalGain { get; set; }

        // 收款方姓名
        public string RecipientName { get; set; }

        // 非应税分配
        public float? NontaxableDistributions { get; set; }

        // 已扣除的联邦收入税
        public float? FederalIncomeTaxWithheld { get; set; }

        // 收款方地址
        public string RecipientAddress { get; set; }

        // 第199A条款股息
        public float? Section199ADividends { get; set; }

        // 投资费用
        public float? InvestmentExpenses { get; set; }

        // 收款方城市
        public string RecipientCity { get; set; }

        // 收款方州
        public string RecipientState { get; set; }

        // 收款方邮政编码
        public string RecipientZIP { get; set; }

        // 收款方的外国国家
        public string RecipientForeignCountry { get; set; }

        // 收款方的外国省份
        public string RecipientForeignProvince { get; set; }

        // 现金清算分配
        public float? CashLiquidationDistr { get; set; }

        // 非现金清算分配
        public float? NoncashLiquidationDistr { get; set; }

        // FATCA报备要求
        public string FATCAFilingRequirement { get; set; }

        // 免税利息股息
        public float? ExemptInterestDividends { get; set; }

        // 私人活动债券利息
        public float? PrivateActivityBondInterest { get; set; }

        // 州
        public string State { get; set; }

        // 账户号码
        public string AccountNumber { get; set; }

        // 第二个TIN通知
        public string SecondTINNotice { get; set; }

        // 支付方州ID号码
        public string PayerStateIDNumber { get; set; }

        // 扣缴的州税
        public float? StateTaxWithheld { get; set; }

        // 外国税款支付
        public float? ForeignTaxPaid { get; set; }

        // 外国国家或美国领土
        public string ForgnCountryOrUSPossession { get; set; }
    }

    public class Dividends1099Generator
    {
        public string GenerateXml(Dividends1099Form form)
        {
            StringBuilder xmlBuilder = new StringBuilder();
            xmlBuilder.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
            xmlBuilder.AppendLine(@"<View xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">");
            xmlBuilder.AppendLine(@"    <Identifier Hierarchy=""Federal\Income\Dividends (IRS 1099-DIV)"" />");
            xmlBuilder.AppendLine(@"    <Controls>");
            xmlBuilder.AppendLine(@"        <Entity ID=""1"" />");
            xmlBuilder.AppendLine(@"    </Controls>");
            xmlBuilder.AppendLine(@"    <WorkSheetSection Name=""Dividends (IRS 1099-DIV)"">");
            xmlBuilder.AppendLine(@"        <GridData ID=""-47020"" Description=""Dividends"">");
            xmlBuilder.AppendLine(@"            <Detail>");
            xmlBuilder.AppendLine(@"                <WorkSheetSection ID=""IFDSDIV"" SectionNumber=""40026"" Name=""IRS 1099-DIV"">");

            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.0", form.TSJ);  // 税务报告类型
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.1", form.FS);  // 报表类型
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.50", form.StateCode);  // 州代码
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.3", form.City);  // 城市名称
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.27", form.OrdinaryDividendsPY);  // 普通股息（前年度）

            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.5", form.PayerName);  // 支付方名称
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.6", form.PayerAddress);  // 支付方地址
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.4", form.OrdinaryDividends.ToString());  // 普通股息
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.7", form.PayerCity);  // 支付方城市
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.32", form.PayerState);  // 支付方州
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.34", form.PayerZIP);  // 支付方邮政编码
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.12", form.PayerTelephone);  // 支付方电话号码

            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.35", form.ForeignCountryProvince);  // 外国/省份
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.33", form.QualifiedDividends.ToString());  // 合格股息
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.36", form.ForeignPostalCode);  // 外国邮政编码
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.37", form.ForeignCountryCode);  // 外国国家代码

            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.8", form.TotalCapitalGainDistr.ToString());  // 总资本增值分配
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.13", form.UnrecapturedSec1250Gain.ToString());  // 未收回的1250节增值
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.15", form.Section1202Gain.ToString());  // 1202节增值
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.9", form.Rate28Gain.ToString());  // 28%税率增值
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.10", form.PayerID);  // 支付方的税务识别号码

            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.11", form.RecipientTIN);  // 收款方税务识别号码
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.52", form.Section897OrdinaryDividends.ToString());  // 第897条款普通股息
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.53", form.Section897CapitalGain.ToString());  // 第897条款资本增值
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.14", form.RecipientName);  // 收款方名称
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.16", form.NontaxableDistributions.ToString());  // 非应税分配

            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.18", form.FederalIncomeTaxWithheld.ToString());  // 已扣除的联邦收入税
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.17", form.RecipientAddress);  // 收款方地址
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.51", form.Section199ADividends.ToString());  // 第199A条款股息
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.19", form.InvestmentExpenses.ToString());  // 投资费用

            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.20", form.RecipientCity);  // 收款方城市
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.38", form.RecipientState);  // 收款方州
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.39", form.RecipientZIP);  // 收款方邮政编码
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.40", form.RecipientForeignCountry);  // 收款方外国国家
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.41", form.RecipientForeignProvince);  // 收款方外国省份

            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.25", form.CashLiquidationDistr.ToString());  // 现金清算分配
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.26", form.NoncashLiquidationDistr.ToString());  // 非现金清算分配

            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.2", form.State);  // 州
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.23", form.AccountNumber);  // 账户号码
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.24", form.SecondTINNotice);  // 第二税号通知
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.44", form.PayerStateIDNumber);  // 支付方州ID号码
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.45", form.StateTaxWithheld.ToString());  // 扣缴的州税

            // 新补充的字段
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.21", form.ForeignTaxPaid.ToString());  // 外国税款支付
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.22", form.ForgnCountryOrUSPossession.ToString());  // 外国国家或美国领土

            // 其他已列出的字段
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.48", form.FATCAFilingRequirement);  // FATCA申报要求
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.42", form.ExemptInterestDividends.ToString());  // 免税股息
            AddFieldIfHasValue(xmlBuilder, "IFDSDIV.43", form.PrivateActivityBondInterest.ToString());  // 私人活动债券利息



            xmlBuilder.AppendLine(@"                </WorkSheetSection>");
            xmlBuilder.AppendLine(@"            </Detail>");
            xmlBuilder.AppendLine(@"        </GridData>");
            xmlBuilder.AppendLine(@"    </WorkSheetSection>");
            xmlBuilder.AppendLine(@"</View>");

            return xmlBuilder.ToString();
        }

        private void AddFieldIfHasValue(StringBuilder xmlBuilder, string fieldId, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                xmlBuilder.AppendLine($@"                    <FieldData Location=""{fieldId}"" LocationType=""FieldID"" Value=""{EscapeXml(value)}"" />");
            }
        }

        private string EscapeXml(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return input
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }
    }

}
