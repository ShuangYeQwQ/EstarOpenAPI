using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Infrastructure.Request.CCHTax
{
    [XmlRoot("TaxReturnBatchEFileExtensionOptions")]
    public class BatchEFileExtensionRequest
    {
        // Unit selection (e.g., FederalAndStates)
        public string UnitSelection { get; set; }

        // Upload options (e.g., UploadAndHold)
        public string UploadOptions { get; set; }

        // Client data options for the tax return
        public string TaxReturnPrintClientDataOptions { get; set; }

        // Print copy options
        public TaxReturnPrintCopyOption TaxReturnPrintCopyOption { get; set; }

        // Device ID
        public string DeviceID { get; set; }

        // Device serial number
        public string DeviceSerialNumber { get; set; }

        // Client's time zone
        public string ClientTimeZone { get; set; }

        // Whether ELF1040 extension includes amount
        public bool Elf1040ExtWithAmount { get; set; }
    }

    public class TaxReturnPrintCopyOption
    {
        // Whether to print to PDF
        public bool PrintToPdf { get; set; }

        // Watermark text or type
        public string Watermark { get; set; }
    }
}
