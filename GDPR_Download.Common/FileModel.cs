using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR_Download.Common
{
    public class FileModel
    {
        [Key]
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTime UploadDate { get; set; }
        public bool Opened { get; set; } = false;
        public DateTime? OpenedDate { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
    }
}
