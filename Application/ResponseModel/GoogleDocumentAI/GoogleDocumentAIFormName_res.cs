using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseModel.GoogleDocumentAI
{
    public class GoogleDocumentAIFormName_res
    {
        //User_Service_File ID
        public string Id { get; set; }
        //文件名
        public string FileName { get; set; }
        //文件网络地址
        public string FileURL { get; set; }
        //存储桶名字
        public string BucketName { get; set; }
        //文件类型  pdf/img
        public string FileType { get; set; }
        //文件存储桶名地址
        public string FileAddress { get; set; }
        //文件内表格文件类型  W-2,1095
        public string FormType { get; set; }
        //文件大小
        public string FileSize { get; set; }
        //FileAddress,FileName,FileType,BucketName

    }
}
