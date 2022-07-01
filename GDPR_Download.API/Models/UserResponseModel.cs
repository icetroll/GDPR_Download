using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GDPR_Download.API.Models
{
    public class UserResponseModel
    {
        public List<UserModel> Users { get; set; }
        public int totalCount { get; set; }
        public int PageNumber { get; set; }
    }

    public class UserModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Roles { get; set; }
    }
}