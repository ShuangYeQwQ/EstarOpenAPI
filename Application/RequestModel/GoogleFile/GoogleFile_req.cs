using Application.RequestModel.PayPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.RequestModel.GoogleFile
{

    /// <summary>
    /// 上传文件至googlecloudstorage
    /// </summary>
    public class UploadGoogleFile_req
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string FileSize { get; set; }
    }
}
