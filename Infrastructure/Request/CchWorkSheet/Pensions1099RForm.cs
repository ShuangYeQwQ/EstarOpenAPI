using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Request.CchWorkSheet
{
    public class Pensions1099RForm
    {
        // 基础字段
        public string TSJ { get; set; }
        public string FS { get; set; }
        public string City { get; set; }
        public decimal? GrossDistributionPY { get; set; }

        // 支付方信息
        public string PayerName { get; set; }
        public string PayerStreetAddress { get; set; }
        public string PayerCity { get; set; }
        public string PayerState { get; set; }
        public string PayerZIP { get; set; }
        public string PayerForeignCountryCode { get; set; }
        public string PayerProvince { get; set; }
        public string PayerTelephone { get; set; }
        public string PayerForeignPhone { get; set; }
        public string PayerFedID { get; set; }

        // 分配金额信息
        public decimal? GrossDistribution { get; set; }
        public decimal? TaxableAmount { get; set; }
        public string TaxableAmountNotDetermined { get; set; }
        public string TotalDistribution { get; set; }
        public decimal? CapitalGain { get; set; }
        public decimal? FederalIncomeTaxWithheld { get; set; }
        public decimal? EmployeeContribution { get; set; }
        public decimal? UnrealizedAppreciation { get; set; }
        public decimal? OtherDistribution { get; set; }
        public decimal? DistributionPercent { get; set; }
        public decimal? PercentOfTotalDistribution { get; set; }
        public decimal? TotalEmployeeContributions { get; set; }
        public decimal? AmountAllocableToIRR { get; set; }
        public int? FirstYearOfRoth401k { get; set; }

        // 收款方信息
        public string RecipientID { get; set; }
        public string RecipientName { get; set; }
        public string RecipientAddress { get; set; }
        public string RecipientCity { get; set; }
        public string RecipientState { get; set; }
        public string RecipientZIP { get; set; }
        public string RecipientForeignCountryCode { get; set; }
        public string RecipientProvince { get; set; }

        // 分配信息
        public string DistributionCode { get; set; }
        public string IRASEPIndicator { get; set; }

        // 州和地方税务信息
        public decimal? StateTaxWithheld { get; set; }
        public string State { get; set; }
        public string PayerStateNumber { get; set; }
        public decimal? StateDistribution { get; set; }
        public decimal? LocalTaxWithheld { get; set; }
        public string LocalityName { get; set; }
        public decimal? LocalDistribution { get; set; }

        // 其他信息
        public string FATCAFilingRequirement { get; set; }
        public string AccountNumber { get; set; }
        public DateTime? DateOfPayment { get; set; }
    }

    public class Pensions1099RGenerator
    {
        public string GenerateXml(Pensions1099RForm form)
        {
            StringBuilder xmlBuilder = new StringBuilder();
            xmlBuilder.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
            xmlBuilder.AppendLine(@"<View xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:type=""Worksheet"">");
            xmlBuilder.AppendLine(@"    <Identifier Hierarchy=""Federal\Income\IRAs, Pensions and Annuities (1099-R)"" />");
            xmlBuilder.AppendLine(@"    <Controls>");
            xmlBuilder.AppendLine(@"        <Entity ID=""1"" />");
            xmlBuilder.AppendLine(@"    </Controls>");
            xmlBuilder.AppendLine(@"    <WorkSheetSection  Name=""Distributions from Pensions, Annuities and IRAs (IRS 1099-R)"">");
            xmlBuilder.AppendLine(@"        <GridData ID=""-47255"" Description=""Dist Pensions, Annuities, IRAs "">");
            xmlBuilder.AppendLine(@"            <Detail>");
            xmlBuilder.AppendLine(@"                <WorkSheetSection Name=""IRS 1099-R"">");

            // 添加所有字段数据（仅当有值时生成）
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.0", form.TSJ);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.1", form.FS);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.3", form.City);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.39", form.GrossDistributionPY?.ToString());

            // 支付方信息
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.4", form.PayerName);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.6", form.PayerStreetAddress);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.8", form.PayerCity);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.44", form.PayerState);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.45", form.PayerZIP);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.48", form.PayerForeignCountryCode);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.51", form.PayerProvince);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.36", form.PayerTelephone);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.37", form.PayerForeignPhone);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.11", form.PayerFedID);

            // 分配金额信息
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.5", form.GrossDistribution?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.7", form.TaxableAmount?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.9", form.TaxableAmountNotDetermined);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.10", form.TotalDistribution);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.13", form.CapitalGain?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.14", form.FederalIncomeTaxWithheld?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.15", form.EmployeeContribution?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.16", form.UnrealizedAppreciation?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.22", form.OtherDistribution?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.23", form.DistributionPercent?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.24", form.PercentOfTotalDistribution?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.25", form.TotalEmployeeContributions?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.50", form.AmountAllocableToIRR?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.43", form.FirstYearOfRoth401k?.ToString());

            // 收款方信息
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.12", form.RecipientID);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.17", form.RecipientName);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.18", form.RecipientAddress);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.19", form.RecipientCity);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.46", form.RecipientState);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.47", form.RecipientZIP);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.49", form.RecipientForeignCountryCode);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.52", form.RecipientProvince);

            // 分配信息
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.20", form.DistributionCode);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.21", form.IRASEPIndicator);

            // 州和地方税务信息
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.26", form.StateTaxWithheld?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.2", form.State);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.27", form.PayerStateNumber);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.28", form.StateDistribution?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.33", form.LocalTaxWithheld?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.34", form.LocalityName);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.35", form.LocalDistribution?.ToString());

            // 其他信息
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.31", form.FATCAFilingRequirement);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.29", form.AccountNumber);
            AddFieldIfHasValue(xmlBuilder, "IFDS099R.32", form.DateOfPayment?.ToString("MM/dd/yyyy"));

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
