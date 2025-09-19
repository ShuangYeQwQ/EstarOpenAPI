using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Request.CchWorkSheet
{
    public class Interest1099_INTForm
    {
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
    public class Interest1099_INTGenerator
    {
        public string GenerateXml(Interest1099_INTForm form)
        {
            StringBuilder xmlBuilder = new StringBuilder();
            xmlBuilder.AppendLine(@"<View xsi:type=""Worksheet"" >");
            xmlBuilder.AppendLine(@"    <Identifier Hierarchy=""Federal\Income\Interest (IRS 1099-INT)"" />");
            xmlBuilder.AppendLine(@"    <Controls>");
            xmlBuilder.AppendLine(@"        <Entity ID=""1"" />");
            xmlBuilder.AppendLine(@"    </Controls>");
            xmlBuilder.AppendLine(@"<WorkSheetSection Name=""Interest (IRS 1099-INT)"">");
            xmlBuilder.AppendLine(@"    <GridData ID=""-47010"" Description=""Interest"">");
            xmlBuilder.AppendLine(@"        <Detail>");
            xmlBuilder.AppendLine(@"            <WorkSheetSection Name=""IRS 1099-INT"">");

            // 添加字段数据 - 仅当有值时生成
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.10", form.InterestIncome?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.12", form.EarlyWithdrawalPenalty?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.13", form.InterestOnUSBonds?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.15", form.FederalIncomeTaxWithheld?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.16", form.InvestmentExpenses?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.20", form.ForeignTaxPaid?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.21", form.ForeignCountryOrUSTerritory);
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.28", form.TaxExemptInterest?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.30", form.SpecifiedPrivateActivityBondInterest?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.47", form.MarketDiscount?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.48", form.BondPremium?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.53", form.BondPremiumOnTreasuryObligations?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.52", form.BondPremiumOnTaxExemptBond?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.35", form.TaxExemptBondCUSIP);
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.55", form.State);
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.42", form.StateIdentificationNo);
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.44", form.StateTaxWithheld?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.5", form.PayerName);
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.6", form.PayerStreetAddress);
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.7", form.PayerCity);
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.31", form.PayerState);
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.32", form.PayerZIP);
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.29", form.PayerTelephone);
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.8", form.PayerTIN);
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.9", form.RecipientTIN);
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.11", form.RecipientName);
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.14", form.RecipientStreetAddress);
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.17", form.RecipientCity);
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.38", form.RecipientState);
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.39", form.RecipientZIP);
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.18", form.AccountNumber);
            AddFieldIfHasValue(xmlBuilder, "IFDSINT.51", form.FATCAFilingRequirement?.ToString().ToLower());

            xmlBuilder.AppendLine(@"            </WorkSheetSection>");
            xmlBuilder.AppendLine(@"        </Detail>");
            xmlBuilder.AppendLine(@"    </GridData>");
            xmlBuilder.AppendLine(@"</WorkSheetSection>");
            xmlBuilder.AppendLine(@"</View>");
            return xmlBuilder.ToString();
        }

        private void AddFieldIfHasValue(StringBuilder xmlBuilder, string fieldId, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                xmlBuilder.AppendLine($@"                <FieldData Location=""{fieldId}"" LocationType=""FieldID"" Value=""{EscapeXml(value)}"" />");
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