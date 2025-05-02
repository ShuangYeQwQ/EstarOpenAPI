using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Response.CCHTax
{
    public class BatchOutputFilesResponse
    {
        /// <summary>
        /// 批处理项列表
        /// </summary>
        public List<BatchOutputFile> Files { get; set; } = new List<BatchOutputFile>();
    }
    public class BatchOutputFile
    {
        /// <summary>
        /// 唯一的批处理项 GUID
        /// </summary>
        public string BatchItemGuid { get; set; } = string.Empty;

        /// <summary>
        /// 生成的文件名称
        /// </summary>
        public string FileName { get; set; } = string.Empty;
    }

   
}
