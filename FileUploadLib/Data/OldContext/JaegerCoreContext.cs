using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FileUploadLib.Data.OldContext
{
    public partial class JaegerCoreContext : DbContext
    {
        public JaegerCoreContext()
        {
        }

        public JaegerCoreContext(DbContextOptions<JaegerCoreContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ansprechpartner> Ansprechpartner { get; set; }
        public virtual DbSet<Applications> Applications { get; set; }
        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Cmsdomains> Cmsdomains { get; set; }
        public virtual DbSet<Cmsentities> Cmsentities { get; set; }
        public virtual DbSet<Cmsmenus> Cmsmenus { get; set; }
        public virtual DbSet<CmspageContent2> CmspageContent2 { get; set; }
        public virtual DbSet<CmspageTypes> CmspageTypes { get; set; }
        public virtual DbSet<Cmspages2> Cmspages2 { get; set; }
        public virtual DbSet<Cmsplaceholder> Cmsplaceholder { get; set; }
        public virtual DbSet<FileToModule> FileToModule { get; set; }
        public virtual DbSet<Files2> Files2 { get; set; }
        public virtual DbSet<HeaderFooterItems> HeaderFooterItems { get; set; }
        public virtual DbSet<Jobs> Jobs { get; set; }
        public virtual DbSet<Memberships> Memberships { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<NewsCategory> NewsCategory { get; set; }
        public virtual DbSet<Newsletter> Newsletter { get; set; }
        public virtual DbSet<NewsletterComponents> NewsletterComponents { get; set; }
        public virtual DbSet<NewsletterComponentsTemplates> NewsletterComponentsTemplates { get; set; }
        public virtual DbSet<NewsletterDistributionLists> NewsletterDistributionLists { get; set; }
        public virtual DbSet<NewsletterLog> NewsletterLog { get; set; }
        public virtual DbSet<NewsletterRecipients> NewsletterRecipients { get; set; }
        public virtual DbSet<NewsletterSpool> NewsletterSpool { get; set; }
        public virtual DbSet<NewsletterSpoolArchive> NewsletterSpoolArchive { get; set; }
        public virtual DbSet<NewsletterTemplates> NewsletterTemplates { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductCategory> ProductCategory { get; set; }
        public virtual DbSet<Profiles> Profiles { get; set; }
        public virtual DbSet<Projects> Projects { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }
        public virtual DbSet<ShortArticleList> ShortArticleList { get; set; }
        public virtual DbSet<TagToModule> TagToModule { get; set; }
        public virtual DbSet<Tags> Tags { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<UsersDetail> UsersDetail { get; set; }
        public virtual DbSet<UsersInRoles> UsersInRoles { get; set; }
        public virtual DbSet<UsersOpenAuthAccounts> UsersOpenAuthAccounts { get; set; }
        public virtual DbSet<UsersOpenAuthData> UsersOpenAuthData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.\\SQL2014;Database=JaegerCore;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Ansprechpartner>(entity =>
            {
                entity.Property(e => e.EditedBy).HasMaxLength(50);

                entity.Property(e => e.EditedDate).HasColumnType("datetime");

                entity.Property(e => e.WrittenBy).HasMaxLength(50);

                entity.Property(e => e.WrittenDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Applications>(entity =>
            {
                entity.HasKey(e => e.ApplicationId)
                    .HasName("PK__Applicat__C93A4C99F2059E6D");

                entity.Property(e => e.ApplicationId).ValueGeneratedNever();

                entity.Property(e => e.ApplicationName)
                    .IsRequired()
                    .HasMaxLength(235);

                entity.Property(e => e.Description).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.HasIndex(e => e.RoleId);

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName)
                    .HasName("RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<Cmsdomains>(entity =>
            {
                entity.HasKey(e => e.DomainId)
                    .HasName("PK_Domains");

                entity.ToTable("CMSDomains");

                entity.Property(e => e.DomainId).HasColumnName("DomainID");

                entity.Property(e => e.Domain).HasMaxLength(250);

                entity.Property(e => e.LocalDomain).HasMaxLength(250);
            });

            modelBuilder.Entity<Cmsentities>(entity =>
            {
                entity.HasKey(e => e.EntityId)
                    .HasName("PK_Entities");

                entity.ToTable("CMSEntities");

                entity.Property(e => e.EntityId).HasColumnName("EntityID");

                entity.Property(e => e.Culture).HasMaxLength(50);

                entity.Property(e => e.DomainId).HasColumnName("DomainID");

                entity.Property(e => e.EntityName).HasMaxLength(250);

                entity.Property(e => e.MasterPage).HasMaxLength(50);

                entity.Property(e => e.Uiculture)
                    .HasColumnName("UICulture")
                    .HasMaxLength(50);

                entity.HasOne(d => d.Domain)
                    .WithMany(p => p.Cmsentities)
                    .HasForeignKey(d => d.DomainId)
                    .HasConstraintName("FK_CMSEntities_CMSDomains");
            });

            modelBuilder.Entity<Cmsmenus>(entity =>
            {
                entity.HasKey(e => e.CmsmenuId);

                entity.ToTable("CMSMenus");

                entity.Property(e => e.CmsmenuId)
                    .HasColumnName("CMSMenuID")
                    .ValueGeneratedNever();

                entity.Property(e => e.CmsentityId).HasColumnName("CMSEntityID");

                entity.Property(e => e.CmsmenuDescription)
                    .HasColumnName("CMSMenuDescription")
                    .HasMaxLength(500);

                entity.Property(e => e.CmsmenuName)
                    .HasColumnName("CMSMenuName")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<CmspageContent2>(entity =>
            {
                entity.HasKey(e => e.PageContentId);

                entity.ToTable("CMSPageContent2");

                entity.HasIndex(e => e.PageId);

                entity.Property(e => e.PageContentId).HasColumnName("PageContentID");

                entity.Property(e => e.EditedBy).HasMaxLength(50);

                entity.Property(e => e.EditedDate).HasColumnType("datetime");

                entity.Property(e => e.Language).HasMaxLength(50);

                entity.Property(e => e.PageId).HasColumnName("PageID");

                entity.Property(e => e.PlaceholderId).HasColumnName("PlaceholderID");

                entity.Property(e => e.WrittenBy).HasMaxLength(50);

                entity.Property(e => e.WrittenDate).HasColumnType("datetime");

                entity.HasOne(d => d.Page)
                    .WithMany(p => p.CmspageContent2)
                    .HasForeignKey(d => d.PageId);
            });

            modelBuilder.Entity<CmspageTypes>(entity =>
            {
                entity.HasKey(e => e.CmspageTypeId)
                    .HasName("PK_CMSTypes");

                entity.ToTable("CMSPageTypes");

                entity.Property(e => e.CmspageTypeId)
                    .HasColumnName("CMSPageTypeId")
                    .ValueGeneratedNever();

                entity.Property(e => e.CmspageTypeName)
                    .HasColumnName("CMSPageTypeName")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Cmspages2>(entity =>
            {
                entity.HasKey(e => e.SitemapExternId);

                entity.ToTable("CMSPages2");

                entity.Property(e => e.SitemapExternId).HasColumnName("SitemapExternID");

                entity.Property(e => e.BelongsToPageId).HasColumnName("BelongsToPageID");

                entity.Property(e => e.EditedBy).HasMaxLength(50);

                entity.Property(e => e.EditedDate).HasColumnType("datetime");

                entity.Property(e => e.EntityId).HasColumnName("EntityID");

                entity.Property(e => e.FriendlyUrl).HasMaxLength(1000);

                entity.Property(e => e.KeywordUrl).HasMaxLength(500);

                entity.Property(e => e.Language).HasMaxLength(50);

                entity.Property(e => e.LinkTitle)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.MenuId).HasColumnName("MenuID");

                entity.Property(e => e.MetaAuthor).HasMaxLength(200);

                entity.Property(e => e.MetaCopyright).HasMaxLength(200);

                entity.Property(e => e.MetaDescription).HasMaxLength(300);

                entity.Property(e => e.MetaIndexFollow).HasMaxLength(50);

                entity.Property(e => e.MetaKeywords).HasMaxLength(2500);

                entity.Property(e => e.MetaPublisher).HasMaxLength(200);

                entity.Property(e => e.MetaTitle).HasMaxLength(500);

                entity.Property(e => e.ModulePageLinkExternal).HasMaxLength(250);

                entity.Property(e => e.ModulePageLinkInternal).HasMaxLength(250);

                entity.Property(e => e.ModuleXmlLink).HasMaxLength(50);

                entity.Property(e => e.ParentId).HasColumnName("ParentID");

                entity.Property(e => e.Priority).HasMaxLength(50);

                entity.Property(e => e.RedirectToPageId).HasColumnName("RedirectToPageID");

                entity.Property(e => e.Roles).HasMaxLength(500);

                entity.Property(e => e.TemplateId).HasColumnName("TemplateID");

                entity.Property(e => e.WrittenBy).HasMaxLength(50);

                entity.Property(e => e.WrittenDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Cmsplaceholder>(entity =>
            {
                entity.HasKey(e => e.PlaceholderId);

                entity.ToTable("CMSPlaceholder");

                entity.Property(e => e.PlaceholderId).HasColumnName("PlaceholderID");

                entity.Property(e => e.CmsplaceholderName)
                    .HasColumnName("CMSPlaceholderName")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<FileToModule>(entity =>
            {
                entity.Property(e => e.FileToModuleId).HasColumnName("FileToModuleID");

                entity.Property(e => e.AltText).HasMaxLength(250);

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.DetailId).HasColumnName("DetailID");

                entity.Property(e => e.EditedBy).HasMaxLength(250);

                entity.Property(e => e.EditedDate).HasColumnType("datetime");

                entity.Property(e => e.FileId).HasColumnName("FileID");

                entity.Property(e => e.InsertedBy).HasMaxLength(250);

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.Legend).HasMaxLength(250);

                entity.Property(e => e.LinkFromPic).HasMaxLength(250);

                entity.Property(e => e.LinkTitle).HasMaxLength(250);

                entity.Property(e => e.LinkToPic).HasMaxLength(250);

                entity.Property(e => e.ModuleId).HasColumnName("ModuleID");

                entity.HasOne(d => d.File)
                    .WithMany(p => p.FileToModule)
                    .HasForeignKey(d => d.FileId)
                    .HasConstraintName("FK_FileToModule_Files21");
            });

            modelBuilder.Entity<Files2>(entity =>
            {
                entity.HasKey(e => e.FileId);

                entity.Property(e => e.FileId).HasColumnName("FileID");

                entity.Property(e => e.DescriptionDe)
                    .HasColumnName("DescriptionDE")
                    .HasMaxLength(250);

                entity.Property(e => e.DescriptionEn)
                    .HasColumnName("DescriptionEN")
                    .HasMaxLength(250);

                entity.Property(e => e.EditedBy).HasMaxLength(50);

                entity.Property(e => e.EditedDate).HasColumnType("datetime");

                entity.Property(e => e.FileA).HasMaxLength(250);

                entity.Property(e => e.FileB).HasMaxLength(250);

                entity.Property(e => e.FileC).HasMaxLength(250);

                entity.Property(e => e.FileD).HasMaxLength(500);

                entity.Property(e => e.FileE).HasMaxLength(500);

                entity.Property(e => e.FileF).HasMaxLength(500);

                entity.Property(e => e.FilePath).HasMaxLength(250);

                entity.Property(e => e.Guid).HasMaxLength(50);

                entity.Property(e => e.OriginalFileNameA).HasMaxLength(250);

                entity.Property(e => e.OriginalFileNameB).HasMaxLength(250);

                entity.Property(e => e.OriginalFileNameC).HasMaxLength(250);

                entity.Property(e => e.OriginalFileNameD).HasMaxLength(250);

                entity.Property(e => e.OriginalFileNameE).HasMaxLength(250);

                entity.Property(e => e.OriginalFileNameF).HasMaxLength(250);

                entity.Property(e => e.TagsDe)
                    .HasColumnName("TagsDE")
                    .HasMaxLength(1000);

                entity.Property(e => e.TagsEn)
                    .HasColumnName("TagsEN")
                    .HasMaxLength(1000);

                entity.Property(e => e.WrittenBy).HasMaxLength(50);

                entity.Property(e => e.WrittenDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<HeaderFooterItems>(entity =>
            {
                entity.HasKey(e => e.ItemId);

                entity.HasIndex(e => e.PageId);

                entity.HasOne(d => d.Page)
                    .WithMany(p => p.HeaderFooterItems)
                    .HasForeignKey(d => d.PageId);
            });

            modelBuilder.Entity<Jobs>(entity =>
            {
                entity.HasKey(e => e.JobId);
            });

            modelBuilder.Entity<Memberships>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__Membersh__1788CC4CFF21152C");

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.Property(e => e.Comment).HasMaxLength(256);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.FailedPasswordAnswerAttemptWindowsStart).HasColumnType("datetime");

                entity.Property(e => e.FailedPasswordAttemptWindowStart).HasColumnType("datetime");

                entity.Property(e => e.LastLockoutDate).HasColumnType("datetime");

                entity.Property(e => e.LastLoginDate).HasColumnType("datetime");

                entity.Property(e => e.LastPasswordChangedDate).HasColumnType("datetime");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.PasswordAnswer).HasMaxLength(128);

                entity.Property(e => e.PasswordQuestion).HasMaxLength(256);

                entity.Property(e => e.PasswordSalt)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.Memberships)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("MembershipApplication");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Memberships)
                    .HasForeignKey<Memberships>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("MembershipUser");
            });

            modelBuilder.Entity<News>(entity =>
            {
                entity.Property(e => e.NewsId).HasColumnName("NewsID");

                entity.Property(e => e.EditedBy).HasMaxLength(50);

                entity.Property(e => e.EditedDate).HasColumnType("datetime");

                entity.Property(e => e.EntityId).HasColumnName("EntityID");

                entity.Property(e => e.Language).HasMaxLength(50);

                entity.Property(e => e.NewsBelongsToNewsId).HasColumnName("NewsBelongsToNewsID");

                entity.Property(e => e.NewsCategoryId).HasColumnName("NewsCategoryID");

                entity.Property(e => e.NewsDateEnd).HasColumnType("datetime");

                entity.Property(e => e.NewsDateStart).HasColumnType("datetime");

                entity.Property(e => e.NewsDateText).HasMaxLength(500);

                entity.Property(e => e.NewsHeadline).HasMaxLength(500);

                entity.Property(e => e.NewsLocation).HasMaxLength(150);

                entity.Property(e => e.NewsPath).HasMaxLength(250);

                entity.Property(e => e.NewsSubline).HasMaxLength(500);

                entity.Property(e => e.NewsTeaser).HasMaxLength(1500);

                entity.Property(e => e.NewsValidFrom).HasColumnType("datetime");

                entity.Property(e => e.NewsValidTo).HasColumnType("datetime");

                entity.Property(e => e.WrittenBy).HasMaxLength(50);

                entity.Property(e => e.WrittenDate).HasColumnType("datetime");

                entity.HasOne(d => d.NewsCategory)
                    .WithMany(p => p.News)
                    .HasForeignKey(d => d.NewsCategoryId)
                    .HasConstraintName("FK_News_NewsCategory1");
            });

            modelBuilder.Entity<NewsCategory>(entity =>
            {
                entity.Property(e => e.NewsCategoryId).HasColumnName("NewsCategoryID");

                entity.Property(e => e.CategoryName).HasMaxLength(250);
            });

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

                entity.Property(e => e.EntityId).HasColumnName("EntityID");

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

                entity.Property(e => e.EntityId).HasColumnName("EntityID");

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

                entity.Property(e => e.EntityId).HasColumnName("EntityID");

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

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.Application).HasMaxLength(1500);

                entity.Property(e => e.ArticleName).HasMaxLength(250);

                entity.Property(e => e.ArticleNo).HasMaxLength(50);

                entity.Property(e => e.BendingLargerThanMm)
                    .HasColumnName("BendingLargerThanMM")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.CornerJoint).HasMaxLength(250);

                entity.Property(e => e.Depth).HasColumnType("decimal(18, 1)");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.DistanceBetweenIlluminants).HasMaxLength(250);

                entity.Property(e => e.EditedBy).HasMaxLength(50);

                entity.Property(e => e.EditedDate).HasColumnType("datetime");

                entity.Property(e => e.MaxSize).HasMaxLength(250);

                entity.Property(e => e.ProductCategoryId).HasColumnName("ProductCategoryID");

                entity.Property(e => e.Reinforcement).HasMaxLength(250);

                entity.Property(e => e.Surface).HasMaxLength(250);

                entity.Property(e => e.WrittenBy).HasMaxLength(50);

                entity.Property(e => e.WrittenDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.Property(e => e.ProductCategoryId).HasColumnName("ProductCategoryID");

                entity.Property(e => e.EditedBy).HasMaxLength(50);

                entity.Property(e => e.EditedDate).HasColumnType("datetime");

                entity.Property(e => e.EntityId).HasColumnName("EntityID");

                entity.Property(e => e.Language).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.Property(e => e.WrittenBy).HasMaxLength(50);

                entity.Property(e => e.WrittenDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Profiles>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__Profiles__1788CC4CBB994E3A");

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.Property(e => e.LastUpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.PropertyNames)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.PropertyValueBinary)
                    .IsRequired()
                    .HasColumnType("image");

                entity.Property(e => e.PropertyValueStrings)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Profiles)
                    .HasForeignKey<Profiles>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("UserProfile");
            });

            modelBuilder.Entity<Projects>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.SizeOfEnterprise).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.HasKey(e => e.RoleId)
                    .HasName("PK__Roles__8AFACE1A8A5B924A");

                entity.Property(e => e.RoleId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasMaxLength(256);

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.Roles)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RoleApplication");
            });

            modelBuilder.Entity<Settings>(entity =>
            {
                entity.HasKey(e => e.SettingId);

                entity.Property(e => e.MetaIcbm).HasColumnName("MetaICBM");
            });

            modelBuilder.Entity<ShortArticleList>(entity =>
            {
                entity.HasKey(e => e.ShortArticleId);

                entity.Property(e => e.ShortArticleId).HasColumnName("ShortArticleID");

                entity.Property(e => e.Articlename).HasMaxLength(250);

                entity.Property(e => e.Category).HasMaxLength(150);

                entity.Property(e => e.Condition).HasMaxLength(1500);

                entity.Property(e => e.Depth).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.EditedBy).HasMaxLength(50);

                entity.Property(e => e.EditedDate).HasColumnType("datetime");

                entity.Property(e => e.EntityId).HasColumnName("EntityID");

                entity.Property(e => e.HoursUsed).HasMaxLength(50);

                entity.Property(e => e.Kw)
                    .HasColumnName("KW")
                    .HasMaxLength(50);

                entity.Property(e => e.Language).HasMaxLength(50);

                entity.Property(e => e.Length).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Model).HasMaxLength(250);

                entity.Property(e => e.OperatingPressure).HasMaxLength(50);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Producer).HasMaxLength(250);

                entity.Property(e => e.ShortArticleBelongsToShortArticleId).HasColumnName("ShortArticleBelongsToShortArticleID");

                entity.Property(e => e.Volume).HasMaxLength(50);

                entity.Property(e => e.Weight).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Width).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.WrittenBy).HasMaxLength(50);

                entity.Property(e => e.WrittenDate).HasColumnType("datetime");

                entity.Property(e => e.YearOfManufacture).HasMaxLength(50);
            });

            modelBuilder.Entity<TagToModule>(entity =>
            {
                entity.Property(e => e.TagToModuleId).HasColumnName("TagToModuleID");

                entity.Property(e => e.DetailId).HasColumnName("DetailID");

                entity.Property(e => e.ModuleId).HasColumnName("ModuleID");

                entity.Property(e => e.TagId).HasColumnName("TagID");
            });

            modelBuilder.Entity<Tags>(entity =>
            {
                entity.HasKey(e => e.TagId);

                entity.Property(e => e.TagId).HasColumnName("TagID");

                entity.Property(e => e.EditedBy).HasMaxLength(50);

                entity.Property(e => e.EditedDate).HasColumnType("datetime");

                entity.Property(e => e.TagName).HasMaxLength(250);

                entity.Property(e => e.WrittenBy).HasMaxLength(50);

                entity.Property(e => e.WrittenDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__Users__1788CC4C3FF776E0");

                entity.HasIndex(e => e.UserName)
                    .HasName("IDX_UserName");

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.Property(e => e.LastActivityDate).HasColumnType("datetime");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("UserApplication");
            });

            modelBuilder.Entity<UsersDetail>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .ValueGeneratedNever();

                entity.Property(e => e.AccountEmail).HasMaxLength(150);

                entity.Property(e => e.ApplicationId).HasColumnName("ApplicationID");

                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.Property(e => e.City).HasMaxLength(150);

                entity.Property(e => e.Company).HasMaxLength(250);

                entity.Property(e => e.CompanyWebSite).HasMaxLength(150);

                entity.Property(e => e.Country).HasMaxLength(150);

                entity.Property(e => e.DeletedReason).HasMaxLength(500);

                entity.Property(e => e.Department).HasMaxLength(150);

                entity.Property(e => e.EditedBy).HasMaxLength(250);

                entity.Property(e => e.EditedDate).HasColumnType("datetime");

                entity.Property(e => e.FaxBusiness).HasMaxLength(150);

                entity.Property(e => e.Field1).HasMaxLength(500);

                entity.Property(e => e.Field1String).HasMaxLength(500);

                entity.Property(e => e.Field2).HasMaxLength(500);

                entity.Property(e => e.Field2String).HasMaxLength(500);

                entity.Property(e => e.Field3).HasMaxLength(500);

                entity.Property(e => e.Field3String).HasMaxLength(500);

                entity.Property(e => e.Field4).HasMaxLength(500);

                entity.Property(e => e.Field4String).HasMaxLength(500);

                entity.Property(e => e.Field5).HasMaxLength(500);

                entity.Property(e => e.Field5String).HasMaxLength(500);

                entity.Property(e => e.FirstName).HasMaxLength(150);

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.InsertedBy).HasMaxLength(250);

                entity.Property(e => e.InsertedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastName).HasMaxLength(150);

                entity.Property(e => e.Limit).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NewsletterNewsletterConfirmedDate).HasColumnType("datetime");

                entity.Property(e => e.OtherBusinessEmail).HasMaxLength(150);

                entity.Property(e => e.OtherPrivateEmail).HasMaxLength(10);

                entity.Property(e => e.PhoneBusiness).HasMaxLength(150);

                entity.Property(e => e.PhoneMobile).HasMaxLength(150);

                entity.Property(e => e.PhonePrivate).HasMaxLength(10);

                entity.Property(e => e.Picture).HasMaxLength(500);

                entity.Property(e => e.Position).HasMaxLength(150);

                entity.Property(e => e.PreferredLanguage).HasMaxLength(50);

                entity.Property(e => e.Salutation).HasMaxLength(50);

                entity.Property(e => e.SortId).HasColumnName("SortID");

                entity.Property(e => e.Street).HasMaxLength(150);

                entity.Property(e => e.StreetNumber).HasMaxLength(50);

                entity.Property(e => e.Titel).HasMaxLength(150);

                entity.Property(e => e.Zip)
                    .HasColumnName("ZIP")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<UsersInRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId })
                    .HasName("PK__UsersInR__AF2760ADA9E103A4");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UsersInRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("UsersInRoleRole");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UsersInRoles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("UsersInRoleUser");
            });

            modelBuilder.Entity<UsersOpenAuthAccounts>(entity =>
            {
                entity.HasKey(e => new { e.ApplicationName, e.ProviderName, e.ProviderUserId })
                    .HasName("PK_dbo.UsersOpenAuthAccounts");

                entity.Property(e => e.ApplicationName).HasMaxLength(128);

                entity.Property(e => e.ProviderName).HasMaxLength(128);

                entity.Property(e => e.ProviderUserId).HasMaxLength(128);

                entity.Property(e => e.LastUsedUtc).HasColumnType("datetime");

                entity.Property(e => e.MembershipUserName)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.ProviderUserName).IsRequired();

                entity.HasOne(d => d.UsersOpenAuthData)
                    .WithMany(p => p.UsersOpenAuthAccounts)
                    .HasForeignKey(d => new { d.ApplicationName, d.MembershipUserName })
                    .HasConstraintName("FK_dbo.UsersOpenAuthAccounts_dbo.UsersOpenAuthData_ApplicationName_MembershipUserName");
            });

            modelBuilder.Entity<UsersOpenAuthData>(entity =>
            {
                entity.HasKey(e => new { e.ApplicationName, e.MembershipUserName })
                    .HasName("PK_dbo.UsersOpenAuthData");

                entity.Property(e => e.ApplicationName).HasMaxLength(128);

                entity.Property(e => e.MembershipUserName).HasMaxLength(128);
            });
        }
    }
}
