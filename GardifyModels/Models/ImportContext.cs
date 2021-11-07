using Microsoft.AspNet.Identity.EntityFramework;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class ImportContext : IdentityDbContext<ApplicationUser>
    {
        public ImportContext()
            : base("ImportConnection", throwIfV1Schema: false)
        {
        }

        public virtual DbSet<SimplePlant> SimplePlant { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}