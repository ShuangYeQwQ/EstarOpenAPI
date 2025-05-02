using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Infrastructure.Request.CCHTax
{
    [XmlRoot("TaxReturnBatchPrintEntireReturn")]
    public class PrintBatchRequest
    {
        // Print client data options
        public string TaxReturnPrintClientDataOptions { get; set; }

        // Return authorization options
        public string ReturnAuthorizationOptions { get; set; }

        // Print set name
        public string PrintsetName { get; set; }

        // Print entire return options
        public PrintEntireReturnOptions PrintEntireReturnOptions { get; set; }
    }

    public class PrintEntireReturnOptions
    {
        // Print one statement per page option
        public bool PrintOneStatementPerPage { get; set; }

        // Create separate files option
        public bool CreateSeparateFiles { get; set; }

        // Print statements behind forms option
        public bool PrintStatementsBehindForms { get; set; }

        // Diagnostics print filter
        public string DiagnosticsPrintFilter { get; set; }

        // Create separate K1 PDF files option
        public bool CreateSeparateK1PDFFiles { get; set; }

        // Print options for accountant copy
        public PrintOptionsAccountantCopy PrintOptions_AccountantCopy { get; set; }
    }

    public class PrintOptionsAccountantCopy
    {
        // Print to PDF option
        public bool PrintToPdf { get; set; }

        // Watermark option
        public string Watermark { get; set; }

        // Print tickmarks option
        public bool PrintTickmarks { get; set; }

        // Is masked option
        public bool IsMasked { get; set; }

        // Include e-file attachment option
        public bool IncludeEFileAttachment { get; set; }
    }
}
