using GDPR_Download.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR_Download.Data
{
    public class SiteDbContext : DbContext
    {
        /* HERE IS A EXAMPLE OF ADDING A MODEL
        public virtual DbSet<Status> Status { get; set; }*/

        public DbSet<FileModel> Files { get; set; }

        public SiteDbContext() : base(nameOrConnectionString: @"DefaultConnection")
        {
            
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }
    }
}
