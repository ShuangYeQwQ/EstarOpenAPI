using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Response.CCHTax
{
    public class ElfStatusByClientResponse
    {
        public int ElfSDKStatus { get; set; }
        public int RecordCount { get; set; }
        public List<ElfErrorMessageResponse> Error { get; set; }
        public List<ElfStatusListResponse> ElfStatusList { get; set; }
        public class ElfErrorMessageResponse
        {
            public string ErrorCode { get; set; }
            public string ErrorDescription { get; set; }
        }
        public class ElfStatusListResponse
        {
            public int AccountNo { get; set; }
            public string BankInfo { get; set; }
            public string CalcVersion { get; set; }
            public string CategoryofReturn { get; set; }
            public string DueDate { get; set; }
            public string EinSsn { get; set; }
            public string ElecDebitElecDeposit { get; set; }
            public decimal ElecDebitElecDepositAmount { get; set; }
            public string ESign { get; set; }
            public string FBarBsaID { get; set; }
            public string FiscalYearBeginDate { get; set; }
            public string FiscalYearEndDate { get; set; }
            public int RecordCount { get; set; }
            public decimal RefundDue { get; set; }
            public Guid ReturnGuid { get; set; }
            public string ReturnId { get; set; }
            public string SignFormReceivedDate { get; set; }
            public string Source { get; set; }
            public int Status { get; set; }
            public string StatusDate { get; set; }
            public string SubmissionID { get; set; }
            public string SummaryStatus { get; set; }
            public string Tin { get; set; }
            public int TypeOfReturn { get; set; }
            public string Unit { get; set; }
            public string UnitType { get; set; }
        }

    }
}
