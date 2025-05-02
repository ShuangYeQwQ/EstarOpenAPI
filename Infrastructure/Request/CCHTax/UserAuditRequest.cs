using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Request.CCHTax
{
    public class UserAuditRequest
    {
        /// <summary>
        /// User Id used to login to CCH Axcess. If the account is using federated authentication,
        /// this will be your organization user name.
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Start date.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// List of Activities.
        /// </summary>
        public List<Activities>? Activities { get; set; }
    }

    public enum Activities
    {
        ELF8955SSASENTTOEFS,
        IXEXPAPP,
        IXIMPAPP,
        RBP,
        IXEXPBSSPRO,
        IXIMPBSSPRO,
        IXEXPBNA,
        CAL,
        COC,
        CHI,
        CHO,
        IXEXPCDX,
        IXIMPCDX,
        CTR,
        CRT,
        DC,
        DELETEDRTNRESTORED,
        IXEXPDPL,
        IXIMPDPL,
        ELFUPLOADEXCEPTION,
        ELFUPLOADSUCCESSFUL,
        ELFEXTSENTTOEFS,
        ELFEXTUPLOADTOEFSBAT,
        ELFEXTAMTSTOEFSBAT,
        ELFFBARSENTTOEFS,
        IXIMPFA,
        IXEXPFE,
        IXIMPFE,
        RSP,
        GDI,
        GDP,
        GDT,
        GDD,
        GTT,
        IXEXPGNSKPR,
        IXIMPGNSKPR,
        IXGLBCWTBIMP,
        IXA2TIMPCASP,
        IXA2TIMPCAFP,
        IXGLBCSATBIMP,
        IXA2TIMPENG,
        IXA2TIMPGNRC,
        IXA2TQL,
        IXGLBTBIMP,
        IXGLIMPCOA,
        IXGLACTV,
        IXGLPRGE,
        IXGLDATAXFER,
        IXGLDACTV,
        IXGLIMPTJE,
        IXGLDQL,
        IXNONE,
        IPD,
        ISP,
        IXIMPB2B,
        IXIMPKSRS,
        IXEXPB2B,
        IXEXPKSRS,
        NOTESPURGED,
        ELFNY204LLSENTTOEFS,
        OP,
        RP,
        PCR,
        IXEXPPLAN,
        RTNREBUILDPERFORMED,
        RCD,
        RR,
        RRS,
        RCP,
        RETDEL,
        RLK,
        RETOPEN,
        RETOPENMU,
        RETPWDREM,
        RETPWDRESET,
        RTNREBUILD,
        RTNSCRUBBED,
        RETSECURED,
        ELFRETSENTTOEFS,
        IXEXPRTU,
        RUK,
        IXIMPSAGE,
        SNT,
        IXEXPTD,
        IXIMPTD,
        TEQPERFORMED,
        TEQRTNCREATED,
        IXEXPTAXSCRPT,
        IXEXPGINX,
        IXIMPGINX,
        TFW,
        TI,
        TTM,
        TTW,
        ULC,
        ELFUPLOADSUCCESSWARN,
        IXEXPVG,
        IXIMPVG,
        IXEXPWKS,
        IXIMPWKS
    }
}
