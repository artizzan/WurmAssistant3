using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using AldursLab.WurmAssistantWebService.Model.Entities;
using AldursLab.WurmAssistantWebService.Model.Entities.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AldursLab.WurmAssistantWebService.Model
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public DbSet<File> Files { get; set; }
        public DbSet<WurmAssistantPackage> WurmAssistantPackages { get; set; }
    }
}