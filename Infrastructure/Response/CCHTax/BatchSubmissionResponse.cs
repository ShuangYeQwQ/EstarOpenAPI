using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Response.CCHTax
{
    public class BatchSubmissionResponse
    {
        /// <summary>
        /// Unique execution ID for the batch submission.
        /// </summary>
        public string ExecutionID { get; set; } = string.Empty;

        /// <summary>
        /// List of results for each submitted file.
        /// </summary>
        public List<BatchFileResult> BatchFileResults { get; set; } = new List<BatchFileResult>();
    }
    public class BatchFileResult
    {
        /// <summary>
        /// Indicates whether there was an error in processing.
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// List of messages related to the submission status.
        /// </summary>
        public List<string> Messages { get; set; } = new List<string>();

        /// <summary>
        /// List of execution IDs for sub-items.
        /// </summary>
        public List<string> SubItemExecutionIDs { get; set; } = new List<string>();

        /// <summary>
        /// Group ID for the file.
        /// </summary>
        public int FileGroupID { get; set; }
    }
}
