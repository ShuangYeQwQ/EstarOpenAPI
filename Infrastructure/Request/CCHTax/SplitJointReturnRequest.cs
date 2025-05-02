using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Request.CCHTax
{
    public class SplitJointReturnRequest
    {
        /// <summary>
        /// 1040 joint return Id, which has to be split.
        /// </summary>
        public string? ReturnId { get; set; }

        /// <summary>
        /// Spouse Client Id.
        /// </summary>
        public string? SpouseClientId { get; set; }

        /// <summary>
        /// Spouse First Name.
        /// </summary>
        public string? SpouseFirstName { get; set; }

        /// <summary>
        /// Option to create a new return or overwrite an existing one.
        /// </summary>
        public ReturnCreationOption ReturnCreationOption { get; set; }
    }

    public enum ReturnCreationOption
    {
        CreateNewVersion,
        OverwriteExistingVersion
    }
}
