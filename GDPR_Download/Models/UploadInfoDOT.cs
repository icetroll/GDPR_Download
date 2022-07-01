namespace GDPR_Download.Models
{
    public class UploadInfoDOT
    {
        public string SenderEmail { get; set; }

        public string ReciverEmail { get; set; }

        public string Subject { get; set; }

        public string Description { get; set; }

        public string FileName { get; set; }

        public bool Sent { get; set; }
    }
}