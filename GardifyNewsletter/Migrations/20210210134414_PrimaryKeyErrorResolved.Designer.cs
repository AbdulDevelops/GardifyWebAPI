﻿// <auto-generated />
using System;
using GardifyNewsletter.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GardifyNewsletter.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20210210134414_PrimaryKeyErrorResolved")]
    partial class PrimaryKeyErrorResolved
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GardifyNewsletter.Areas.Identity.Data.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("GardifyNewsletter.Models.Newsletter", b =>
                {
                    b.Property<int>("NewsletterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("NewsletterID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool?>("Active");

                    b.Property<string>("ApplicationId");

                    b.Property<bool?>("Deleted");

                    b.Property<string>("EditedBy")
                        .HasMaxLength(150);

                    b.Property<DateTime?>("EditedDate")
                        .HasColumnType("datetime");

                    b.Property<string>("NewsletterCompleteHtml")
                        .HasColumnName("NewsletterCompleteHTML");

                    b.Property<string>("NewsletterDateShownOnNewsletter")
                        .HasMaxLength(250);

                    b.Property<string>("NewsletterHeaderText")
                        .HasMaxLength(250);

                    b.Property<string>("NewsletterInternalName")
                        .HasMaxLength(250);

                    b.Property<string>("NewsletterMainPicLink")
                        .HasMaxLength(250);

                    b.Property<DateTime?>("NewsletterSentDate");

                    b.Property<string>("NewsletterStatus")
                        .HasMaxLength(150);

                    b.Property<int?>("NewsletterTemplateId")
                        .HasColumnName("NewsletterTemplateID");

                    b.Property<bool?>("NotEditable");

                    b.Property<string>("SenderEmail")
                        .HasMaxLength(250);

                    b.Property<string>("SenderName")
                        .HasMaxLength(250);

                    b.Property<string>("SenderReplyTo")
                        .HasMaxLength(250);

                    b.Property<int?>("Sort");

                    b.Property<string>("Subject")
                        .HasMaxLength(250);

                    b.Property<string>("WrittenBy")
                        .HasMaxLength(150);

                    b.Property<DateTime?>("WrittenDate")
                        .HasColumnType("datetime");

                    b.HasKey("NewsletterId");

                    b.ToTable("Newsletter");
                });

            modelBuilder.Entity("GardifyNewsletter.Models.NewsletterComponents", b =>
                {
                    b.Property<int>("NewsletterComponentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("NewsletterComponentID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool?>("Active");

                    b.Property<string>("ApplicationId")
                        .HasColumnName("EntityID");

                    b.Property<int?>("BelongsToNewsletterId")
                        .HasColumnName("BelongsToNewsletterID");

                    b.Property<string>("CustomLinkText");

                    b.Property<bool?>("Deleted");

                    b.Property<string>("EditedBy")
                        .HasMaxLength(150);

                    b.Property<DateTime?>("EditedDate")
                        .HasColumnType("datetime");

                    b.Property<int?>("NewsId");

                    b.Property<string>("NewsleterComponentText");

                    b.Property<string>("NewsletterComponentHeadline")
                        .HasMaxLength(200);

                    b.Property<string>("NewsletterComponentSubline")
                        .HasMaxLength(200);

                    b.Property<int?>("NewsletterComponentTemplateId")
                        .HasColumnName("NewsletterComponentTemplateID");

                    b.Property<string>("NewsletterMoreLink")
                        .HasMaxLength(250);

                    b.Property<string>("NewsletterPicLink")
                        .HasMaxLength(250);

                    b.Property<bool?>("NotEditable");

                    b.Property<int?>("ShortArticleId");

                    b.Property<int?>("Sort");

                    b.Property<string>("WrittenBy")
                        .HasMaxLength(150);

                    b.Property<DateTime?>("WrittenDate")
                        .HasColumnType("datetime");

                    b.HasKey("NewsletterComponentId");

                    b.HasIndex("BelongsToNewsletterId");

                    b.ToTable("NewsletterComponents");
                });

            modelBuilder.Entity("GardifyNewsletter.Models.NewsletterComponentsTemplates", b =>
                {
                    b.Property<int>("NewsletterComponentsTemplateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("NewsletterComponentsTemplateID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool?>("Active");

                    b.Property<bool?>("Deleted");

                    b.Property<string>("EditedBy")
                        .HasMaxLength(150);

                    b.Property<DateTime?>("EditedDate")
                        .HasColumnType("datetime");

                    b.Property<string>("NewsletterComponentName")
                        .HasMaxLength(250);

                    b.Property<bool?>("NotEditable");

                    b.Property<int?>("Sort");

                    b.Property<string>("WrittenBy")
                        .HasMaxLength(150);

                    b.Property<DateTime?>("WrittenDate")
                        .HasColumnType("datetime");

                    b.HasKey("NewsletterComponentsTemplateId");

                    b.ToTable("NewsletterComponentsTemplates");
                });

            modelBuilder.Entity("GardifyNewsletter.Models.NewsletterDistributionLists", b =>
                {
                    b.Property<int>("NewsletterDistributionListId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("NewsletterDistributionListID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool?>("Active");

                    b.Property<string>("ApplicationId")
                        .HasColumnName("EntityID");

                    b.Property<bool?>("Deleted");

                    b.Property<string>("EditedBy")
                        .HasMaxLength(150);

                    b.Property<DateTime?>("EditedDate")
                        .HasColumnType("datetime");

                    b.Property<int?>("LanguageId")
                        .HasColumnName("LanguageID");

                    b.Property<string>("NewsletterDistributionListName")
                        .HasMaxLength(250);

                    b.Property<bool?>("NotEditable");

                    b.Property<int?>("Sort");

                    b.Property<string>("WrittenBy")
                        .HasMaxLength(150);

                    b.Property<DateTime?>("WrittenDate")
                        .HasColumnType("datetime");

                    b.HasKey("NewsletterDistributionListId");

                    b.ToTable("NewsletterDistributionLists");
                });

            modelBuilder.Entity("GardifyNewsletter.Models.NewsletterLog", b =>
                {
                    b.Property<int>("NewsletterLogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("NewsletterLogID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("NewsletterLogDatum")
                        .HasColumnType("datetime");

                    b.Property<string>("NewsletterLogText")
                        .HasMaxLength(500);

                    b.HasKey("NewsletterLogId");

                    b.ToTable("NewsletterLog");
                });

            modelBuilder.Entity("GardifyNewsletter.Models.NewsletterNewPlants", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("NewPlant1");

                    b.Property<string>("NewPlant1SubHeadline");

                    b.Property<int?>("NewPlant2");

                    b.Property<string>("NewPlant2SubHeadline");

                    b.Property<int?>("NewPlant3");

                    b.Property<string>("NewPlant3SubHeadline");

                    b.Property<int>("NewPlantComponentId");

                    b.Property<string>("NewPlantMonth");

                    b.HasKey("Id");

                    b.HasIndex("NewPlantComponentId")
                        .IsUnique();

                    b.ToTable("NewsletterNewPlants");
                });

            modelBuilder.Entity("GardifyNewsletter.Models.NewsletterRecipients", b =>
                {
                    b.Property<int>("NewsletterRecipientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("NewsletterRecipientID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool?>("Active");

                    b.Property<string>("ApplicationId")
                        .HasColumnName("EntityID");

                    b.Property<bool?>("Confirmed");

                    b.Property<bool?>("Deleted");

                    b.Property<bool?>("Editable");

                    b.Property<string>("EditedBy")
                        .HasMaxLength(150);

                    b.Property<DateTime?>("EditedDate")
                        .HasColumnType("datetime");

                    b.Property<string>("FlagPromotionKindOf")
                        .HasMaxLength(1000);

                    b.Property<string>("ImportedBy")
                        .HasMaxLength(150);

                    b.Property<int?>("LanguageId")
                        .HasColumnName("LanguageID");

                    b.Property<string>("MailComesFromThisExternalList")
                        .HasMaxLength(250);

                    b.Property<int?>("NewsletterDistributionListId")
                        .HasColumnName("NewsletterDistributionListID");

                    b.Property<string>("RecipientEmail")
                        .HasColumnName("RecipientEMail")
                        .HasMaxLength(250);

                    b.Property<string>("RecipientName")
                        .HasMaxLength(500);

                    b.Property<Guid?>("RegistrationCode");

                    b.Property<DateTime?>("RegistrationDate")
                        .HasColumnType("datetime");

                    b.Property<int?>("Sort");

                    b.Property<string>("WrittenBy")
                        .HasMaxLength(150);

                    b.Property<DateTime?>("WrittenDate")
                        .HasColumnType("datetime");

                    b.HasKey("NewsletterRecipientId");

                    b.HasIndex("NewsletterDistributionListId");

                    b.ToTable("NewsletterRecipients");
                });

            modelBuilder.Entity("GardifyNewsletter.Models.NewsletterSpool", b =>
                {
                    b.Property<int>("NewsletterSpoolId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("NewsletterSpoolID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("AddedToSpool")
                        .HasColumnType("datetime");

                    b.Property<string>("Body");

                    b.Property<string>("Credentials")
                        .HasMaxLength(250);

                    b.Property<string>("FromEmail")
                        .HasMaxLength(250);

                    b.Property<string>("FromName")
                        .HasMaxLength(250);

                    b.Property<string>("FromReplyTo")
                        .HasMaxLength(250);

                    b.Property<bool?>("Html")
                        .HasColumnName("HTML");

                    b.Property<int?>("NewsletterDistributionListId")
                        .HasColumnName("NewsletterDistributionListID");

                    b.Property<int?>("NewsletterId")
                        .HasColumnName("NewsletterID");

                    b.Property<int?>("Port");

                    b.Property<string>("RecipientEmail")
                        .HasMaxLength(250);

                    b.Property<int?>("RecipientId")
                        .HasColumnName("RecipientID");

                    b.Property<DateTime?>("Scheduled")
                        .HasColumnType("datetime");

                    b.Property<bool?>("Send");

                    b.Property<string>("SenderDomain")
                        .HasMaxLength(250);

                    b.Property<string>("Subject")
                        .HasMaxLength(500);

                    b.Property<bool?>("Subscriber");

                    b.Property<Guid?>("UserId")
                        .HasColumnName("UserID");

                    b.HasKey("NewsletterSpoolId");

                    b.ToTable("NewsletterSpool");
                });

            modelBuilder.Entity("GardifyNewsletter.Models.NewsletterSpoolArchive", b =>
                {
                    b.Property<int>("NewsletterSpoolArchiveId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("NewsletterSpoolArchiveID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("AddedToSpool")
                        .HasColumnType("datetime");

                    b.Property<string>("Body");

                    b.Property<string>("Credentials")
                        .HasMaxLength(250);

                    b.Property<string>("FromEmail")
                        .HasMaxLength(250);

                    b.Property<string>("FromName")
                        .HasMaxLength(250);

                    b.Property<string>("FromReplyTo")
                        .HasMaxLength(250);

                    b.Property<bool?>("Html")
                        .HasColumnName("HTML");

                    b.Property<int?>("NewsletterDistributionListId")
                        .HasColumnName("NewsletterDistributionListID");

                    b.Property<int?>("NewsletterId")
                        .HasColumnName("NewsletterID");

                    b.Property<int?>("Port");

                    b.Property<string>("RecipientEmail")
                        .HasMaxLength(250);

                    b.Property<int?>("RecipientId")
                        .HasColumnName("RecipientID");

                    b.Property<DateTime?>("Scheduled")
                        .HasColumnType("datetime");

                    b.Property<bool?>("Send");

                    b.Property<DateTime?>("SendedDate")
                        .HasColumnType("datetime");

                    b.Property<string>("SenderDomain")
                        .HasMaxLength(250);

                    b.Property<string>("Subject")
                        .HasMaxLength(500);

                    b.Property<Guid?>("UserId")
                        .HasColumnName("UserID");

                    b.HasKey("NewsletterSpoolArchiveId");

                    b.ToTable("NewsletterSpoolArchive");
                });

            modelBuilder.Entity("GardifyNewsletter.Models.NewsletterTemplates", b =>
                {
                    b.Property<int>("NewsletterTemplateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("NewsletterTemplateID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool?>("Active");

                    b.Property<bool?>("Deleted");

                    b.Property<string>("EditedBy")
                        .HasMaxLength(150);

                    b.Property<DateTime?>("EditedDate")
                        .HasColumnType("datetime");

                    b.Property<string>("NewsletterTemplateFooterHtml")
                        .HasColumnName("NewsletterTemplateFooterHTML")
                        .HasMaxLength(2000);

                    b.Property<string>("NewsletterTemplateHeaderHtml")
                        .HasColumnName("NewsletterTemplateHeaderHTML")
                        .HasMaxLength(2000);

                    b.Property<string>("NewsletterTemplateName")
                        .HasMaxLength(250);

                    b.Property<bool?>("NotEditable");

                    b.Property<int?>("Sort");

                    b.Property<string>("WrittenBy")
                        .HasMaxLength(150);

                    b.Property<DateTime?>("WrittenDate")
                        .HasColumnType("datetime");

                    b.HasKey("NewsletterTemplateId");

                    b.ToTable("NewsletterTemplates");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("GardifyNewsletter.Models.NewsletterComponents", b =>
                {
                    b.HasOne("GardifyNewsletter.Models.Newsletter", "Newsletter")
                        .WithMany("NewsletterComponents")
                        .HasForeignKey("BelongsToNewsletterId");
                });

            modelBuilder.Entity("GardifyNewsletter.Models.NewsletterNewPlants", b =>
                {
                    b.HasOne("GardifyNewsletter.Models.NewsletterComponents", "NewsletterComponents")
                        .WithOne("NewsletterNewPlants")
                        .HasForeignKey("GardifyNewsletter.Models.NewsletterNewPlants", "NewPlantComponentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GardifyNewsletter.Models.NewsletterRecipients", b =>
                {
                    b.HasOne("GardifyNewsletter.Models.NewsletterDistributionLists", "NewsletterDistributionList")
                        .WithMany("Recipients")
                        .HasForeignKey("NewsletterDistributionListId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("GardifyNewsletter.Areas.Identity.Data.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("GardifyNewsletter.Areas.Identity.Data.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GardifyNewsletter.Areas.Identity.Data.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("GardifyNewsletter.Areas.Identity.Data.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
