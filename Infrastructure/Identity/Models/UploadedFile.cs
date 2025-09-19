using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity.Models
{
    public class UploadedFile
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = "anonymous";
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Content { get; set; }
        public string FileType { get; set; }
        public string Source { get; set; } = "upload";
        public DateTime UploadTime { get; set; }
    }

}
