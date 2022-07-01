using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR_Download.Common
{
    public class GuestLoginModel
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Mail { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Sent { get; set; }
        public Guid LoginToken { get; set; }

    }
}
