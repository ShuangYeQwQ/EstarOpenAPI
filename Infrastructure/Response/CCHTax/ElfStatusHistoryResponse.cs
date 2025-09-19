using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Response.CCHTax
{
    public class ElfStatusHistoryResponse
    {
        public string BankInfo { get; set; }
        public string CategoryOfReturn { get; set; }
        public string EPostMark { get; set; }
        public string FiscalYearBegin { get; set; }
        public string FiscalYearEnd { get; set; }
        public string Form8879DateReceived { get; set; }
        public string IRSCenter { get; set; }
        public string IRSMessage { get; set; }
        public bool IsPasswordProtected { get; set; }
        public bool IsRefund { get; set; }
        public bool IsSSN { get; set; }
        public string Name { get; set; }
        public string Notification { get; set; }
        public string PlanNumber { get; set; }
        public string PreparerName { get; set; }
        public string Product { get; set; }
        public decimal RefundAmount { get; set; }
        public string ReturnID { get; set; }
        public string SsnEin { get; set; }
        public List<StatusHistory> StatusHistoryList { get; set; }
        public List<SubUnit> SubUnitList { get; set; }
        public int TaxYear { get; set; }
        public string TypeOfReturn { get; set; }
    }

    public class StatusHistory
    {
        public string AckDate { get; set; }
        public string CategoryOfReturn { get; set; }
        public int DisplayCode { get; set; }
        public string Form8879DateReceived { get; set; }
        public string Form8879DateReceived_UpldLevel { get; set; }
        public int intCOR { get; set; }
        public int IsActive { get; set; }
        public bool IsFBAR { get; set; }
        public bool IsParticipatingInEsign { get; set; }
        public string MISBTID { get; set; }
        public string ModifiedByName { get; set; }
        public bool ParentIsPasswordProtected { get; set; }
        public bool ParentWasPasswordProtected { get; set; }
        public string StateCode { get; set; }
        public decimal StateDue { get; set; }
        public string StatusDate { get; set; }
        public int StatusID { get; set; }
        public string StatusText { get; set; }
        public string SubmissionId { get; set; }
        public string UnitName { get; set; }
        public string UnmaskedMISBTID { get; set; }
        public string UnmaskedStateCode { get; set; }
    }

    public class SubUnit
    {
        public string AckBSAID { get; set; }
        public string AckDate { get; set; }
        public string CategoryOfReturn { get; set; }
        public string CorText { get; set; }
        public bool IsFBAR { get; set; }
        public bool ParentIsPasswordProtected { get; set; }
        public bool ParentWasPasswordProtected { get; set; }
        public string StateCode { get; set; }
        public string StatusDate { get; set; }
        public int StatusID { get; set; }
        public string StatusText { get; set; }
        public string SubUnitID { get; set; }
        public string UnmaskedSubUnitID { get; set; }
    }
}
