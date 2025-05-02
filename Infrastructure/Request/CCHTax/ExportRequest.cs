using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Infrastructure.Request.CCHTax
{
    [XmlRoot("TaxDataExportOptions")]
    public class ExportRequest
    {
        // Export units selection preference
        public string ExportUnitsSelectionPreference { get; set; }

        // Default preferences for export
        public DefaultPreferences DefaultPreferences { get; set; }

        // Export diagnostics mode
        public string ExportDiagnosticsMode { get; set; }

        // Whether to calculate return before export
        public bool CalcReturnBeforeExport { get; set; }

        // Default field identifier preference
        public string DefaultFieldIdentifierPreference { get; set; }
    }

    public class DefaultPreferences
    {
        // Whether to generate meta
        public bool GenerateMeta { get; set; }

        // Whether to generate lookup items
        public bool GenerateLookupItems { get; set; }

        // Field value export selection
        public string FieldValueExportSelection { get; set; }

        // Worksheet grid export mode
        public string WorksheetGridExportMode { get; set; }

        // Whitepaper statement export mode
        public string WhitepaperStatementExportMode { get; set; }
    }
}
