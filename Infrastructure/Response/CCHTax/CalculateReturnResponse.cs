using Interfaces.Response.CCHTax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Response.CCHTax
{
    public class CalculateReturnResponse
    {
        public string ExecutionID { get; set; }
        public List<BatchFileResultsResponse> BatchFileResults { get; set; }

        public class BatchFileResultsResponse
        {
            public string IsError { get; set; }
            public List<string> Messages { get; set; }
            public List<string> SubItemExecutionIDs { get; set; }
            public string FileGroupID { get; set; }
        }
        //    "" :  "265118f1-c7fd-48d0-882e-831771eab1c7" , 
        //"BatchFileResults" :  [ { 
        //    "IsError" :  false , 
        //    "Messages" :  ["{ReturnId} 已成功提交。"] , 
        //    "SubItemExecutionIDs" :  ["85046d61-6a8d-4642-a57e-ee38a9896d25"] , 
        //    "FileGroupID" :  0 
        //} ] 
    }
}
