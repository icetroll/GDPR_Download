using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GDPR_Download.API.Models
{
    public class UserInfoModel
    {
    }

    public class UpdateUser
    {
        public string Email { get; set; }
        public string oldEmail { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public bool Admin { get; set; }
        public bool User { get; set; }
    }
}