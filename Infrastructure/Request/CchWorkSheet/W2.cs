using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.X509;
using PaypalServerSdk.Standard.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Infrastructure.Request.CchGenerator
{
    public class W2Form
    {
        // 雇主信息
        public string EmployerName { get; set; } 
        public string EmployerAddress_StreetAddressOrPostalBox { get; set; }
        public string EmployerAddress_City { get; set; }
        public string EmployerAddress_State { get; set; }
        public string EmployerAddress_Zip { get; set; }
        public string EmployerForeignCountryCode { get; set; } // 外国国家代码
        public string EIN { get; set; }  // 雇主识别号

        // 雇员信息
        public string EmployeeName_FirstName { get; set; }
        public string EmployeeName_LastName { get; set; }
        public string EmployeeName_Suffix { get; set; }  // 姓名后缀 (Jr., Sr.等)
        public string EmployeeAddress_StreetAddressOrPostalBox { get; set; }
        public string EmployeeAddress_City { get; set; }
        public string EmployeeAddress_State { get; set; }
        public string EmployeeAddress_Zip { get; set; }
        public string EmployeeForeignCountryCode { get; set; } // 外国国家代码
        public string SSN { get; set; }  // 社会安全号码

        // 税务控制信息
        public string ControlNumber { get; set; } // 控制号

        // 收入与税务扣缴
        public decimal? WagesTipsOtherCompensation { get; set; } // 工资、小费及其他报酬
        public decimal? FederalIncomeTaxWithheld { get; set; } // 联邦税预扣
        public decimal? SocialSecurityWages { get; set; } // 社会安全工资
        public decimal? SocialSecurityTaxWithheld { get; set; } // 社会安全税预扣
        public decimal? MedicareWagesAndTips { get; set; } // 医疗保险工资和小费
        public decimal? MedicareTaxWithheld { get; set; } // 医疗保险税预扣
        public decimal? SocialSecurityTips { get; set; } // 社会安全小费
        public decimal? AllocatedTips { get; set; } // 分配的小费
        public decimal? DependentCareBenefits { get; set; } // 扶养照顾福利
        public decimal? NonqualifiedPlans { get; set; } // 非合格计划

        // 特殊代码区域 (例如 Box 12)
        public string Code12 { get; set; }  // 代码
        public string Code12Value { get; set; }  // 代码的值

        // 复选框标识 (例如是否是法定员工、是否有退休计划等)
        public string StatutoryEmployee { get; set; }  // 法定员工
        public string RetirementPlan { get; set; }  // 退休计划
        public string ThirdPartySickPay { get; set; }  // 第三方病假工资
        public string Other { get; set; }  // 其他标识

        // 州税务信息 (State-specific)
        public string State_Line1 { get; set; }  // 州税务第一行
        public string EmployerStateIdNumber_Line1 { get; set; }  // 雇主州ID号
        public decimal? StateWagesTipsEtc_Line1 { get; set; }  // 州工资、小费等
        public decimal? StateIncomeTax_Line1 { get; set; }  // 州收入税

        // 地方税务信息 (Local Tax Info)
        public decimal? LocalWagesTipsEtc { get; set; }  // 地方工资、小费等
        public decimal? LocalIncomeTax { get; set; }  // 地方收入税
        public string CityCode { get; set; }  //地方代码
        public string LocalityName { get; set; }  // 地方名称

        // 新增的地方税务信息字段
        public decimal? Military { get; set; }  // 军事
        public decimal? StateUse { get; set; }  // 州使用
        public decimal? StateUse2 { get; set; }  // 州使用 2
        public decimal? StateDisabilityInsurance { get; set; }  // 州残疾保险
        public decimal? WorkDaysHoursInCity { get; set; }  // 在城市中的工作天数/小时
        public string DatesWorkedInCityFrom { get; set; }  // 在城市工作的日期从
        public string DatesWorkedInCityTo { get; set; }  // 在城市工作的日期至
        public decimal? HypoStateWagesOverride { get; set; }  // 假设州工资 - 覆盖
        public string HypoStateCode { get; set; }  // 假设州代码（如果不同）
        public string StateExcludeFromHypo { get; set; }  // 从假设州排除的州

        // Box 12 网格数据 (代码与金额)
        public List<string> Box12Codes { get; set; } = new List<string>();
        public List<decimal> Box12Values { get; set; } = new List<decimal>();

        // Box 14 或其他数据（根据需求添加）
        public List<string> OtherCodes { get; set; } = new List<string>();  // 其他代码
        public List<decimal> OtherAmounts { get; set; } = new List<decimal>();  // 其他金额
    }
    public class W2Generator { 
        // 生成XML的方法
        public string GenerateXml(W2Form w2Form)
        {
            StringBuilder xmlBuilder = new StringBuilder();

            // 开始构建XML
            xmlBuilder.AppendLine(@"<View xsi:type=""Worksheet"" >");
            xmlBuilder.AppendLine(@"    <Identifier Hierarchy=""Federal\Income\Wages, Salaries and Tips (W-2)"" />");
            xmlBuilder.AppendLine(@"    <Controls>");
            xmlBuilder.AppendLine(@"        <Entity ID=""1"" />");
            xmlBuilder.AppendLine(@"    </Controls>");
            xmlBuilder.AppendLine(@"    <WorkSheetSection Name=""Wages and Salaries (IRS W-2)"">");
            xmlBuilder.AppendLine(@"        <GridData ID=""-47009"" Description=""Wages and Salaries"">");
            xmlBuilder.AppendLine(@"            <Detail>");
            xmlBuilder.AppendLine(@"                <WorkSheetSection Name=""IRS W-2"">");

            // 添加所有非网格字段数据（仅当有值时添加）
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.0", ""); // 示例字段
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.1", ""); // 示例字段
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.17", w2Form.SSN);
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.2", ""); // 示例字段
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.3", w2Form.EIN);
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.4", w2Form.WagesTipsOtherCompensation.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.5", w2Form.FederalIncomeTaxWithheld?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.6", w2Form.EmployerName);
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.7", w2Form.SocialSecurityWages?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.8", w2Form.SocialSecurityTaxWithheld?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.9", w2Form.EmployerAddress_StreetAddressOrPostalBox);
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.10", w2Form.MedicareWagesAndTips?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.11", w2Form.MedicareTaxWithheld?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.12", w2Form.EmployerAddress_City);
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.13", w2Form.EmployerAddress_State);
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.14", w2Form.EmployerAddress_Zip);
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.45", ""); // 示例字段
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.15", w2Form.SocialSecurityTips?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.16", w2Form.AllocatedTips?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.44", w2Form.ControlNumber);
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.19", w2Form.DependentCareBenefits?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.20", $"{w2Form.EmployeeName_FirstName} {w2Form.EmployeeName_LastName}".Trim());
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.21", w2Form.NonqualifiedPlans?.ToString());
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.30", w2Form.StatutoryEmployee?.ToString().ToLower());
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.31", w2Form.RetirementPlan?.ToString().ToLower());
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.32", w2Form.ThirdPartySickPay?.ToString().ToLower());
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.33", w2Form.EmployeeAddress_StreetAddressOrPostalBox);
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.34", w2Form.EmployeeAddress_City);
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.35", w2Form.EmployeeAddress_State);
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.36", w2Form.EmployeeAddress_Zip);
            AddFieldIfHasValue(xmlBuilder, "IFDSW2.46", ""); // 示例字段

            // 添加Box 12网格
            AddBox12Grid(xmlBuilder, w2Form);

            // 添加Other网格
            AddOtherGrid(xmlBuilder, w2Form);

            // 添加州和地方信息网格
            AddStateCityGrid(xmlBuilder, w2Form);

            // 结束XML构建
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

        private void AddBox12Grid(StringBuilder xmlBuilder, W2Form w2Form)
        {
            var box12code = w2Form.Code12.Split(",");
            var box12value = w2Form.Code12Value.Split(",");
            bool hasData = false;
            for (int i = 0; i < box12code.Length; i++) {
                if (!string.IsNullOrEmpty(box12code[i])) hasData = true;
            }

            if (!hasData) return;

            xmlBuilder.AppendLine($@"                    <GridData ID=""A40025"" Description=""Box 12"">");
            xmlBuilder.AppendLine($@"                        <Fields>");
            xmlBuilder.AppendLine($@"                            <FieldHeader Location=""IFDSW2.22"" LocationType=""FieldID"" FieldIndex=""1"" />");
            xmlBuilder.AppendLine($@"                            <FieldHeader Location=""IFDSW2.23"" LocationType=""FieldID"" FieldIndex=""2"" />");
            xmlBuilder.AppendLine($@"                        </Fields>");

            for (int i = 0; i < box12code.Length; i++)
            {
                if (!string.IsNullOrEmpty(box12code[i]))
                {
                    xmlBuilder.AppendLine($@"                        <Row>");
                    xmlBuilder.AppendLine($@"                            <RowValue FieldIndex=""1"" Value=""{EscapeXml(box12code[i])}"" />");
                    xmlBuilder.AppendLine($@"                            <RowValue FieldIndex=""2"" Value=""{EscapeXml(box12value[i])}"" />");
                    xmlBuilder.AppendLine($@"                        </Row>");
                }
            }

            xmlBuilder.AppendLine($@"                    </GridData>");
        }

        private void AddOtherGrid(StringBuilder xmlBuilder, W2Form w2Form)
        {
            if (string.IsNullOrEmpty(w2Form.Other)) return;

            xmlBuilder.AppendLine($@"                    <GridData ID=""B40025"" Description=""Other"">");
            xmlBuilder.AppendLine($@"                        <Fields>");
            xmlBuilder.AppendLine($@"                            <FieldHeader Location=""IFDSW2.24"" LocationType=""FieldID"" FieldIndex=""1"" />");
            xmlBuilder.AppendLine($@"                            <FieldHeader Location=""IFDSW2.25"" LocationType=""FieldID"" FieldIndex=""2"" />");
            xmlBuilder.AppendLine($@"                            <FieldHeader Location=""IFDSW2.18"" LocationType=""FieldID"" FieldIndex=""3"" />");
            xmlBuilder.AppendLine($@"                        </Fields>");
            xmlBuilder.AppendLine($@"                        <Row>");
            xmlBuilder.AppendLine($@"                            <RowValue FieldIndex=""1"" Value=""{EscapeXml(w2Form.Other)}"" />");
            xmlBuilder.AppendLine($@"                            <RowValue FieldIndex=""2"" Value="""" />");
            xmlBuilder.AppendLine($@"                            <RowValue FieldIndex=""3"" Value="""" />");
            xmlBuilder.AppendLine($@"                        </Row>");
            xmlBuilder.AppendLine($@"                    </GridData>");
        }

        //private void AddStateCityGrid(StringBuilder xmlBuilder, W2Form w2Form)
        //{
        //    bool hasData = !string.IsNullOrEmpty(w2Form.State_Line1) ||
        //                   !string.IsNullOrEmpty(w2Form.EmployerStateIdNumber_Line1) ||
        //                   w2Form.StateWagesTipsEtc_Line1.HasValue ||
        //                   w2Form.StateIncomeTax_Line1.HasValue ||
        //                   w2Form.LocalWagesTipsEtc.HasValue ||
        //                   w2Form.LocalIncomeTax.HasValue ||
        //                   !string.IsNullOrEmpty(w2Form.LocalityName);

        //    if (!hasData) return;

        //    xmlBuilder.AppendLine($@"                    <GridData ID=""C40025"" Description=""State and City Info"">");
        //    xmlBuilder.AppendLine($@"                        <Fields>");
        //    xmlBuilder.AppendLine($@"                            <FieldHeader Location=""IFDSW2.26"" LocationType=""FieldID"" FieldIndex=""1"" />");
        //    xmlBuilder.AppendLine($@"                            <FieldHeader Location=""IFDSW2.27"" LocationType=""FieldID"" FieldIndex=""2"" />");
        //    xmlBuilder.AppendLine($@"                            <FieldHeader Location=""IFDSW2.28"" LocationType=""FieldID"" FieldIndex=""3"" />");
        //    xmlBuilder.AppendLine($@"                            <FieldHeader Location=""IFDSW2.29"" LocationType=""FieldID"" FieldIndex=""4"" />");
        //    xmlBuilder.AppendLine($@"                            <FieldHeader Location=""IFDSW2.37"" LocationType=""FieldID"" FieldIndex=""5"" />");
        //    xmlBuilder.AppendLine($@"                            <FieldHeader Location=""IFDSW2.38"" LocationType=""FieldID"" FieldIndex=""6"" />");
        //    xmlBuilder.AppendLine($@"                            <FieldHeader Location=""IFDSW2.39"" LocationType=""FieldID"" FieldIndex=""7"" />");
        //    xmlBuilder.AppendLine($@"                            <FieldHeader Location=""IFDSW2.40"" LocationType=""FieldID"" FieldIndex=""8"" />");
        //    xmlBuilder.AppendLine($@"                            <FieldHeader Location=""IFDSW2.41"" LocationType=""FieldID"" FieldIndex=""9"" />");
        //    xmlBuilder.AppendLine($@"                            <FieldHeader Location=""IFDSW2.42"" LocationType=""FieldID"" FieldIndex=""10"" />");
        //    xmlBuilder.AppendLine($@"                            <FieldHeader Location=""IFDSW2.51"" LocationType=""FieldID"" FieldIndex=""11"" />");
        //    xmlBuilder.AppendLine($@"                            <FieldHeader Location=""IFDSW2.43"" LocationType=""FieldID"" FieldIndex=""12"" />");
        //    xmlBuilder.AppendLine($@"                            <FieldHeader Location=""IFDSW2.52"" LocationType=""FieldID"" FieldIndex=""13"" />");
        //    xmlBuilder.AppendLine($@"                            <FieldHeader Location=""IFDSW2.49"" LocationType=""FieldID"" FieldIndex=""14"" />");
        //    xmlBuilder.AppendLine($@"                            <FieldHeader Location=""IFDSW2.50"" LocationType=""FieldID"" FieldIndex=""15"" />");
        //    xmlBuilder.AppendLine($@"                            <FieldHeader Location=""IFDSW2.53"" LocationType=""FieldID"" FieldIndex=""16"" />");
        //    xmlBuilder.AppendLine($@"                            <FieldHeader Location=""IFDSW2.47"" LocationType=""FieldID"" FieldIndex=""17"" />");
        //    xmlBuilder.AppendLine($@"                            <FieldHeader Location=""IFDSW2.48"" LocationType=""FieldID"" FieldIndex=""18"" />");
        //    xmlBuilder.AppendLine($@"                        </Fields>");
        //    xmlBuilder.AppendLine($@"                        <Row>");
        //    AddGridValue(xmlBuilder, "1", w2Form.State_Line1);
        //    AddGridValue(xmlBuilder, "2", w2Form.EmployerStateIdNumber_Line1);
        //    AddGridValue(xmlBuilder, "3", w2Form.StateWagesTipsEtc_Line1?.ToString());
        //    AddGridValue(xmlBuilder, "4", w2Form.StateIncomeTax_Line1?.ToString());
        //    AddGridValue(xmlBuilder, "5", w2Form.LocalWagesTipsEtc?.ToString());
        //    AddGridValue(xmlBuilder, "6", w2Form.LocalIncomeTax?.ToString());
        //    AddGridValue(xmlBuilder, "7", w2Form.CityCode); // City code (无数据)
        //    AddGridValue(xmlBuilder, "8", w2Form.LocalityName);

        //    AddGridValue(xmlBuilder, "9", w2Form.Military?.ToString());
        //    AddGridValue(xmlBuilder, "10", w2Form.StateUse?.ToString());
        //    AddGridValue(xmlBuilder, "11", w2Form.StateUse2?.ToString());
        //    AddGridValue(xmlBuilder, "12", w2Form.StateDisabilityInsurance?.ToString());
        //    AddGridValue(xmlBuilder, "13", w2Form.WorkDaysHoursInCity?.ToString());
        //    AddGridValue(xmlBuilder, "14", w2Form.DatesWorkedInCityFrom?.ToString());
        //    AddGridValue(xmlBuilder, "15", w2Form.DatesWorkedInCityTo); // City code (无数据)
        //    AddGridValue(xmlBuilder, "16", w2Form.HypoStateWagesOverride?.ToString());
        //    AddGridValue(xmlBuilder, "17", w2Form.HypoStateCode); // City code (无数据)
        //    AddGridValue(xmlBuilder, "18", w2Form.StateExcludeFromHypo);

        //    xmlBuilder.AppendLine($@"                        </Row>");
        //    xmlBuilder.AppendLine($@"                    </GridData>");
        //}

        private void AddStateCityGrid(StringBuilder xmlBuilder, W2Form w2Form)
        {
            // 用于存储所有需要的字段
            var fieldValues = new List<(string FieldID, object FieldValue)>
    {
        ("IFDSW2.26", w2Form.State_Line1),
        ("IFDSW2.27", w2Form.EmployerStateIdNumber_Line1),
        ("IFDSW2.28", w2Form.StateWagesTipsEtc_Line1),
        ("IFDSW2.29", w2Form.StateIncomeTax_Line1),
        ("IFDSW2.37", w2Form.LocalWagesTipsEtc),
        ("IFDSW2.38", w2Form.LocalIncomeTax),
        ("IFDSW2.39", w2Form.CityCode),
        ("IFDSW2.40", w2Form.LocalityName),
        ("IFDSW2.41", w2Form.Military),
        ("IFDSW2.42", w2Form.StateUse),
        ("IFDSW2.43", w2Form.StateUse2),
        ("IFDSW2.49", w2Form.StateDisabilityInsurance),
        ("IFDSW2.50", w2Form.WorkDaysHoursInCity),
        ("IFDSW2.51", w2Form.DatesWorkedInCityFrom),
        ("IFDSW2.52", w2Form.DatesWorkedInCityTo),
        ("IFDSW2.53", w2Form.HypoStateWagesOverride),
        ("IFDSW2.47", w2Form.HypoStateCode),
        ("IFDSW2.48", w2Form.StateExcludeFromHypo)
    };

            // 筛选出非空字段
            var nonEmptyFields = fieldValues
         .Where(f => f.FieldValue != null &&
                     ((f.FieldValue is decimal d && d != 0) || // 对于 decimal 类型，判断是否为 0
                      (f.FieldValue is string s && !string.IsNullOrEmpty(s))))
         .ToList();

            // 如果没有非空字段，返回
            if (nonEmptyFields.Count == 0) return;

            // 添加GridData
            xmlBuilder.AppendLine($@"                    <GridData ID=""C40025"" Description=""State and City Info"">");
            xmlBuilder.AppendLine($@"                        <Fields>");

            // 添加FieldHeader，并递增FieldIndex
            for (int i = 0; i < nonEmptyFields.Count; i++)
            {
                xmlBuilder.AppendLine($@"                            <FieldHeader Location=""{nonEmptyFields[i].FieldID}"" LocationType=""FieldID"" FieldIndex=""{i + 1}"" />");
            }

            xmlBuilder.AppendLine($@"                        </Fields>");
            xmlBuilder.AppendLine($@"                        <Row>");

            // 添加GridValue（使用优化后的值转换）
            for (int i = 0; i < nonEmptyFields.Count; i++)
            {
                var fieldValue = nonEmptyFields[i].FieldValue;
                string value = "";

                // 更简洁的类型处理
                //switch (fieldValue)
                //{
                //    case decimal? d:
                //        value = d.Value.ToString(CultureInfo.InvariantCulture);
                //        break;
                //    case string s:
                //        value = s;
                //        break;
                //}

                AddGridValue(xmlBuilder, (i + 1).ToString(), value);
            }

            xmlBuilder.AppendLine($@"                        </Row>");
            xmlBuilder.AppendLine($@"                    </GridData>");
        }

        private void AddGridValue(StringBuilder xmlBuilder, string index, string value)
        {
            value = string.IsNullOrEmpty(value) ? "" : EscapeXml(value);
            xmlBuilder.AppendLine($@"                            <RowValue FieldIndex=""{index}"" Value=""{value}"" />");
        }

        private string EscapeXml(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return input.Replace("&", "&amp;")
                        .Replace("<", "&lt;")
                        .Replace(">", "&gt;")
                        .Replace("\"", "&quot;")
                        .Replace("'", "&apos;");
        }

       
    }
}
