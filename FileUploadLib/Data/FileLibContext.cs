using FileUploadLib.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileUploadLib.Data
{
    public class FileLibContext : DbContext
    {
        public UtilityService _utilityService;
        public FileLibContext(DbContextOptions<FileLibContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _utilityService = new UtilityService(httpContextAccessor);
        }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);
        //    builder.Entity<FileToModule>()
        //        .HasOne<File>();
        //    builder.Entity<FileToModule>()
        //        .HasOne<Module>();
        //    // Customize the ASP.NET Identity model and override the defaults if needed.
        //    // For example, you can rename the ASP.NET Identity table names and more.
        //    // Add your customizations after calling base.OnModelCreating(builder);
        //}

        public DbSet<File> Files { get; set; }
        public DbSet<FileToModule> FileToModules { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Module> Modules { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }

        public override int SaveChanges()
        {
            //int
            var addedEntities = ChangeTracker.Entries()
                                    .Where(x => x.Entity is IBaseModel<int> &&
                                    (x.State == EntityState.Added));
            var updatedEntities = ChangeTracker.Entries()
                                    .Where(x => x.Entity is IBaseModel<int> &&
                                    (x.State == EntityState.Modified));
            var deletedEntities = ChangeTracker.Entries()
                                    .Where(x => x.Entity is IBaseModel<int> &&
                                    (x.State == EntityState.Deleted));
            //string
            var addedEntitiesString = ChangeTracker.Entries()
                                    .Where(x => x.Entity is IBaseModel<string> &&
                                    (x.State == EntityState.Added));
            var updatedEntitiesString = ChangeTracker.Entries()
                                    .Where(x => x.Entity is IBaseModel<string> &&
                                    (x.State == EntityState.Modified));
            string userName = _utilityService.GetUserId();

            //int
            foreach (var entity in addedEntities)
            {
                ((IBaseModel<int>)entity.Entity).OnCreate(userName);
            }
            foreach (var entity in updatedEntities)
            {
                ((IBaseModel<int>)entity.Entity).OnEdit(userName);
            }
            foreach (var entity in deletedEntities)
            {
                ((IBaseModel<int>)entity.Entity).OnDelete(userName);
            }
            //string
            foreach (var entity in addedEntitiesString)
            {
                ((IBaseModel<string>)entity.Entity).OnCreate(userName);
            }
            foreach (var entity in updatedEntitiesString)
            {
                ((IBaseModel<string>)entity.Entity).OnEdit(userName);
            }
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var addedEntities = ChangeTracker.Entries()
                                    .Where(x => x.Entity is IBaseModel<int> &&
                                    (x.State == EntityState.Added));
            var updatedEntities = ChangeTracker.Entries()
                                    .Where(x => x.Entity is IBaseModel<int> &&
                                    (x.State == EntityState.Modified));
            var addedEntitiesString = ChangeTracker.Entries()
                                    .Where(x => x.Entity is IBaseModel<string> &&
                                    (x.State == EntityState.Added));
            var updatedEntitiesString = ChangeTracker.Entries()
                                    .Where(x => x.Entity is IBaseModel<string> &&
                                    (x.State == EntityState.Modified));
            var addedEntitiesGuid = ChangeTracker.Entries()
                                    .Where(x => x.Entity is IBaseModel<Guid> &&
                                    (x.State == EntityState.Added));
            var updatedEntitiesGuid = ChangeTracker.Entries()
                                    .Where(x => x.Entity is IBaseModel<Guid> &&
                                    (x.State == EntityState.Modified));
            string userName = _utilityService.GetUserId();

            foreach (var entity in addedEntities)
            {
                ((IBaseModel<int>)entity.Entity).OnCreate(userName);
            }
            foreach (var entity in addedEntitiesString)
            {
                ((IBaseModel<string>)entity.Entity).OnCreate(userName);
            }
            foreach (var entity in addedEntitiesGuid)
            {
                ((IBaseModel<Guid>)entity.Entity).OnCreate(userName);
            }
            foreach (var entity in updatedEntities)
            {
                ((IBaseModel<int>)entity.Entity).OnEdit(userName);
            }
            foreach (var entity in updatedEntitiesString)
            {
                ((IBaseModel<string>)entity.Entity).OnEdit(userName);
            }
            foreach (var entity in updatedEntitiesGuid)
            {
                ((IBaseModel<Guid>)entity.Entity).OnEdit(userName);
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
