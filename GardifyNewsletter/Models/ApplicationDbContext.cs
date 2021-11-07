using System;
using GardifyNewsletter.Areas.Identity.Data;
using GardifyNewsletter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GardifyNewsletter.Models
{
    public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }


        public virtual DbSet<Newsletter> Newsletter { get; set; }
        public virtual DbSet<NewsletterComponents> NewsletterComponents { get; set; }
        public virtual DbSet<NewsletterComponentsTemplates> NewsletterComponentsTemplates { get; set; }
        public virtual DbSet<NewsletterDistributionLists> NewsletterDistributionLists { get; set; }
        public virtual DbSet<NewsletterLog> NewsletterLog { get; set; }
        public virtual DbSet<NewsletterRecipients> NewsletterRecipients { get; set; }
        public virtual DbSet<NewsletterSpool> NewsletterSpool { get; set; }
        public virtual DbSet<NewsletterSpoolArchive> NewsletterSpoolArchive { get; set; }
        public virtual DbSet<NewsletterTemplates> NewsletterTemplates { get; set; }
        public virtual DbSet<NewsletterNewPlants> NewsletterNewPlants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Newsletter>(entity =>
            {
                entity.Property(e => e.NewsletterId).HasColumnName("NewsletterID");

                entity.Property(e => e.EditedBy).HasMaxLength(150);

                entity.Property(e => e.EditedDate).HasColumnType("datetime");

                entity.Property(e => e.NewsletterCompleteHtml).HasColumnName("NewsletterCompleteHTML");

                entity.Property(e => e.NewsletterDateShownOnNewsletter).HasMaxLength(250);

                entity.Property(e => e.NewsletterHeaderText).HasMaxLength(250);

                entity.Property(e => e.NewsletterInternalName).HasMaxLength(250);

                entity.Property(e => e.NewsletterMainPicLink).HasMaxLength(250);

                entity.Property(e => e.NewsletterStatus).HasMaxLength(150);

                entity.Property(e => e.NewsletterTemplateId).HasColumnName("NewsletterTemplateID");

                entity.Property(e => e.SenderEmail).HasMaxLength(250);

                entity.Property(e => e.SenderName).HasMaxLength(250);

                entity.Property(e => e.SenderReplyTo).HasMaxLength(250);

                entity.Property(e => e.Subject).HasMaxLength(250);

                entity.Property(e => e.WrittenBy).HasMaxLength(150);

                entity.Property(e => e.WrittenDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<NewsletterComponents>(entity =>
            {
                entity.HasKey(e => e.NewsletterComponentId);

                entity.Property(e => e.NewsletterComponentId).HasColumnName("NewsletterComponentID");

                entity.Property(e => e.BelongsToNewsletterId).HasColumnName("BelongsToNewsletterID");

                entity.Property(e => e.EditedBy).HasMaxLength(150);

                entity.Property(e => e.EditedDate).HasColumnType("datetime");

                entity.Property(e => e.ApplicationId).HasColumnName("EntityID");

                entity.Property(e => e.NewsletterComponentHeadline).HasMaxLength(200);

                entity.Property(e => e.NewsletterComponentSubline).HasMaxLength(200);

                entity.Property(e => e.NewsletterComponentTemplateId).HasColumnName("NewsletterComponentTemplateID");

                entity.Property(e => e.NewsletterMoreLink).HasMaxLength(250);

                entity.Property(e => e.NewsletterPicLink).HasMaxLength(250);

                entity.Property(e => e.WrittenBy).HasMaxLength(150);

                entity.Property(e => e.WrittenDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<NewsletterComponentsTemplates>(entity =>
            {
                entity.HasKey(e => e.NewsletterComponentsTemplateId);

                entity.Property(e => e.NewsletterComponentsTemplateId).HasColumnName("NewsletterComponentsTemplateID");

                entity.Property(e => e.EditedBy).HasMaxLength(150);

                entity.Property(e => e.EditedDate).HasColumnType("datetime");

                entity.Property(e => e.NewsletterComponentName).HasMaxLength(250);

                entity.Property(e => e.WrittenBy).HasMaxLength(150);

                entity.Property(e => e.WrittenDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<NewsletterDistributionLists>(entity =>
            {
                entity.HasKey(e => e.NewsletterDistributionListId);

                entity.Property(e => e.NewsletterDistributionListId).HasColumnName("NewsletterDistributionListID");

                entity.Property(e => e.EditedBy).HasMaxLength(150);

                entity.Property(e => e.EditedDate).HasColumnType("datetime");

                entity.Property(e => e.ApplicationId).HasColumnName("EntityID");

                entity.Property(e => e.LanguageId).HasColumnName("LanguageID");

                entity.Property(e => e.NewsletterDistributionListName).HasMaxLength(250);

                entity.Property(e => e.WrittenBy).HasMaxLength(150);

                entity.Property(e => e.WrittenDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<NewsletterLog>(entity =>
            {
                entity.Property(e => e.NewsletterLogId).HasColumnName("NewsletterLogID");

                entity.Property(e => e.NewsletterLogDatum).HasColumnType("datetime");

                entity.Property(e => e.NewsletterLogText).HasMaxLength(500);
            });

            modelBuilder.Entity<NewsletterRecipients>(entity =>
            {
                entity.HasKey(e => e.NewsletterRecipientId);

                entity.Property(e => e.NewsletterRecipientId).HasColumnName("NewsletterRecipientID");

                entity.Property(e => e.EditedBy).HasMaxLength(150);

                entity.Property(e => e.EditedDate).HasColumnType("datetime");

                entity.Property(e => e.ApplicationId).HasColumnName("EntityID");

                entity.Property(e => e.FlagPromotionKindOf).HasMaxLength(1000);

                entity.Property(e => e.ImportedBy).HasMaxLength(150);

                entity.Property(e => e.LanguageId).HasColumnName("LanguageID");

                entity.Property(e => e.MailComesFromThisExternalList).HasMaxLength(250);

                entity.Property(e => e.NewsletterDistributionListId).HasColumnName("NewsletterDistributionListID");

                entity.Property(e => e.RecipientEmail)
                    .HasColumnName("RecipientEMail")
                    .HasMaxLength(250);

                entity.Property(e => e.RecipientName).HasMaxLength(500);

                entity.Property(e => e.RegistrationDate).HasColumnType("datetime");

                entity.Property(e => e.WrittenBy).HasMaxLength(150);

                entity.Property(e => e.WrittenDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<NewsletterSpool>(entity =>
            {
                entity.Property(e => e.NewsletterSpoolId).HasColumnName("NewsletterSpoolID");

                entity.Property(e => e.AddedToSpool).HasColumnType("datetime");

                entity.Property(e => e.Credentials).HasMaxLength(250);

                entity.Property(e => e.FromEmail).HasMaxLength(250);

                entity.Property(e => e.FromName).HasMaxLength(250);

                entity.Property(e => e.FromReplyTo).HasMaxLength(250);

                entity.Property(e => e.Html).HasColumnName("HTML");

                entity.Property(e => e.NewsletterDistributionListId).HasColumnName("NewsletterDistributionListID");

                entity.Property(e => e.NewsletterId).HasColumnName("NewsletterID");

                entity.Property(e => e.RecipientEmail).HasMaxLength(250);

                entity.Property(e => e.RecipientId).HasColumnName("RecipientID");

                entity.Property(e => e.Scheduled).HasColumnType("datetime");

                entity.Property(e => e.SenderDomain).HasMaxLength(250);

                entity.Property(e => e.Subject).HasMaxLength(500);

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<NewsletterSpoolArchive>(entity =>
            {
                entity.Property(e => e.NewsletterSpoolArchiveId).HasColumnName("NewsletterSpoolArchiveID");

                entity.Property(e => e.AddedToSpool).HasColumnType("datetime");

                entity.Property(e => e.Credentials).HasMaxLength(250);

                entity.Property(e => e.FromEmail).HasMaxLength(250);

                entity.Property(e => e.FromName).HasMaxLength(250);

                entity.Property(e => e.FromReplyTo).HasMaxLength(250);

                entity.Property(e => e.Html).HasColumnName("HTML");

                entity.Property(e => e.NewsletterDistributionListId).HasColumnName("NewsletterDistributionListID");

                entity.Property(e => e.NewsletterId).HasColumnName("NewsletterID");

                entity.Property(e => e.RecipientEmail).HasMaxLength(250);

                entity.Property(e => e.RecipientId).HasColumnName("RecipientID");

                entity.Property(e => e.Scheduled).HasColumnType("datetime");

                entity.Property(e => e.SendedDate).HasColumnType("datetime");

                entity.Property(e => e.SenderDomain).HasMaxLength(250);

                entity.Property(e => e.Subject).HasMaxLength(500);

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<NewsletterTemplates>(entity =>
            {
                entity.HasKey(e => e.NewsletterTemplateId);

                entity.Property(e => e.NewsletterTemplateId).HasColumnName("NewsletterTemplateID");

                entity.Property(e => e.EditedBy).HasMaxLength(150);

                entity.Property(e => e.EditedDate).HasColumnType("datetime");

                entity.Property(e => e.NewsletterTemplateFooterHtml)
                    .HasColumnName("NewsletterTemplateFooterHTML")
                    .HasMaxLength(2000);

                entity.Property(e => e.NewsletterTemplateHeaderHtml)
                    .HasColumnName("NewsletterTemplateHeaderHTML")
                    .HasMaxLength(2000);

                entity.Property(e => e.NewsletterTemplateName).HasMaxLength(250);

                entity.Property(e => e.WrittenBy).HasMaxLength(150);

                entity.Property(e => e.WrittenDate).HasColumnType("datetime");
            });


            base.OnModelCreating(modelBuilder);
        }
    }
}
