using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GardifyNewsletter.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Newsletter",
                columns: table => new
                {
                    NewsletterID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EntityId = table.Column<int>(nullable: false),
                    NewsletterTemplateID = table.Column<int>(nullable: true),
                    NewsletterInternalName = table.Column<string>(maxLength: 250, nullable: true),
                    NewsletterDateShownOnNewsletter = table.Column<string>(maxLength: 250, nullable: true),
                    NewsletterHeaderText = table.Column<string>(maxLength: 250, nullable: true),
                    NewsletterMainPicLink = table.Column<string>(maxLength: 250, nullable: true),
                    NewsletterStatus = table.Column<string>(maxLength: 150, nullable: true),
                    NewsletterSentDate = table.Column<DateTime>(nullable: true),
                    SenderEmail = table.Column<string>(maxLength: 250, nullable: true),
                    SenderName = table.Column<string>(maxLength: 250, nullable: true),
                    SenderReplyTo = table.Column<string>(maxLength: 250, nullable: true),
                    Subject = table.Column<string>(maxLength: 250, nullable: true),
                    NewsletterCompleteHTML = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    Sort = table.Column<int>(nullable: true),
                    WrittenDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    WrittenBy = table.Column<string>(maxLength: 150, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EditedBy = table.Column<string>(maxLength: 150, nullable: true),
                    NotEditable = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Newsletter", x => x.NewsletterID);
                });

            migrationBuilder.CreateTable(
                name: "NewsletterComponentsTemplates",
                columns: table => new
                {
                    NewsletterComponentsTemplateID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NewsletterComponentName = table.Column<string>(maxLength: 250, nullable: true),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    Sort = table.Column<int>(nullable: true),
                    WrittenDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    WrittenBy = table.Column<string>(maxLength: 150, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EditedBy = table.Column<string>(maxLength: 150, nullable: true),
                    NotEditable = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsletterComponentsTemplates", x => x.NewsletterComponentsTemplateID);
                });

            migrationBuilder.CreateTable(
                name: "NewsletterDistributionLists",
                columns: table => new
                {
                    NewsletterDistributionListID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NewsletterDistributionListName = table.Column<string>(maxLength: 250, nullable: true),
                    EntityID = table.Column<int>(nullable: true),
                    LanguageID = table.Column<int>(nullable: true),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    Sort = table.Column<int>(nullable: true),
                    WrittenDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    WrittenBy = table.Column<string>(maxLength: 150, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EditedBy = table.Column<string>(maxLength: 150, nullable: true),
                    NotEditable = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsletterDistributionLists", x => x.NewsletterDistributionListID);
                });

            migrationBuilder.CreateTable(
                name: "NewsletterLog",
                columns: table => new
                {
                    NewsletterLogID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NewsletterLogText = table.Column<string>(maxLength: 500, nullable: true),
                    NewsletterLogDatum = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsletterLog", x => x.NewsletterLogID);
                });

            migrationBuilder.CreateTable(
                name: "NewsletterSpool",
                columns: table => new
                {
                    NewsletterSpoolID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SenderDomain = table.Column<string>(maxLength: 250, nullable: true),
                    NewsletterID = table.Column<int>(nullable: true),
                    NewsletterDistributionListID = table.Column<int>(nullable: true),
                    RecipientID = table.Column<int>(nullable: true),
                    UserID = table.Column<Guid>(nullable: true),
                    RecipientEmail = table.Column<string>(maxLength: 250, nullable: true),
                    FromEmail = table.Column<string>(maxLength: 250, nullable: true),
                    FromName = table.Column<string>(maxLength: 250, nullable: true),
                    FromReplyTo = table.Column<string>(maxLength: 250, nullable: true),
                    Subject = table.Column<string>(maxLength: 500, nullable: true),
                    Body = table.Column<string>(nullable: true),
                    HTML = table.Column<bool>(nullable: true),
                    Port = table.Column<int>(nullable: true),
                    Scheduled = table.Column<DateTime>(type: "datetime", nullable: true),
                    Credentials = table.Column<string>(maxLength: 250, nullable: true),
                    AddedToSpool = table.Column<DateTime>(type: "datetime", nullable: true),
                    Send = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsletterSpool", x => x.NewsletterSpoolID);
                });

            migrationBuilder.CreateTable(
                name: "NewsletterSpoolArchive",
                columns: table => new
                {
                    NewsletterSpoolArchiveID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SenderDomain = table.Column<string>(maxLength: 250, nullable: true),
                    NewsletterID = table.Column<int>(nullable: true),
                    NewsletterDistributionListID = table.Column<int>(nullable: true),
                    RecipientID = table.Column<int>(nullable: true),
                    UserID = table.Column<Guid>(nullable: true),
                    RecipientEmail = table.Column<string>(maxLength: 250, nullable: true),
                    FromEmail = table.Column<string>(maxLength: 250, nullable: true),
                    FromName = table.Column<string>(maxLength: 250, nullable: true),
                    FromReplyTo = table.Column<string>(maxLength: 250, nullable: true),
                    Subject = table.Column<string>(maxLength: 500, nullable: true),
                    Body = table.Column<string>(nullable: true),
                    HTML = table.Column<bool>(nullable: true),
                    Port = table.Column<int>(nullable: true),
                    Scheduled = table.Column<DateTime>(type: "datetime", nullable: true),
                    Credentials = table.Column<string>(maxLength: 250, nullable: true),
                    AddedToSpool = table.Column<DateTime>(type: "datetime", nullable: true),
                    SendedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Send = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsletterSpoolArchive", x => x.NewsletterSpoolArchiveID);
                });

            migrationBuilder.CreateTable(
                name: "NewsletterTemplates",
                columns: table => new
                {
                    NewsletterTemplateID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NewsletterTemplateName = table.Column<string>(maxLength: 250, nullable: true),
                    NewsletterTemplateHeaderHTML = table.Column<string>(maxLength: 2000, nullable: true),
                    NewsletterTemplateFooterHTML = table.Column<string>(maxLength: 2000, nullable: true),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    Sort = table.Column<int>(nullable: true),
                    WrittenDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    WrittenBy = table.Column<string>(maxLength: 150, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EditedBy = table.Column<string>(maxLength: 150, nullable: true),
                    NotEditable = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsletterTemplates", x => x.NewsletterTemplateID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NewsletterComponents",
                columns: table => new
                {
                    NewsletterComponentID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EntityID = table.Column<int>(nullable: true),
                    BelongsToNewsletterID = table.Column<int>(nullable: true),
                    NewsletterComponentTemplateID = table.Column<int>(nullable: true),
                    NewsletterComponentHeadline = table.Column<string>(maxLength: 200, nullable: true),
                    NewsletterComponentSubline = table.Column<string>(maxLength: 200, nullable: true),
                    NewsleterComponentText = table.Column<string>(nullable: true),
                    NewsletterPicLink = table.Column<string>(maxLength: 250, nullable: true),
                    NewsletterMoreLink = table.Column<string>(maxLength: 250, nullable: true),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    Sort = table.Column<int>(nullable: true),
                    WrittenDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    WrittenBy = table.Column<string>(maxLength: 150, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EditedBy = table.Column<string>(maxLength: 150, nullable: true),
                    NotEditable = table.Column<bool>(nullable: true),
                    NewsId = table.Column<int>(nullable: true),
                    ShortArticleId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsletterComponents", x => x.NewsletterComponentID);
                    table.ForeignKey(
                        name: "FK_NewsletterComponents_Newsletter_BelongsToNewsletterID",
                        column: x => x.BelongsToNewsletterID,
                        principalTable: "Newsletter",
                        principalColumn: "NewsletterID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NewsletterRecipients",
                columns: table => new
                {
                    NewsletterRecipientID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EntityID = table.Column<int>(nullable: true),
                    LanguageID = table.Column<int>(nullable: true),
                    RecipientEMail = table.Column<string>(maxLength: 250, nullable: true),
                    RecipientName = table.Column<string>(maxLength: 500, nullable: true),
                    MailComesFromThisExternalList = table.Column<string>(maxLength: 250, nullable: true),
                    FlagPromotionKindOf = table.Column<string>(maxLength: 1000, nullable: true),
                    ImportedBy = table.Column<string>(maxLength: 150, nullable: true),
                    NewsletterDistributionListID = table.Column<int>(nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    RegistrationCode = table.Column<Guid>(nullable: true),
                    Confirmed = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    Active = table.Column<bool>(nullable: true),
                    WrittenDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    WrittenBy = table.Column<string>(maxLength: 150, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EditedBy = table.Column<string>(maxLength: 150, nullable: true),
                    Sort = table.Column<int>(nullable: true),
                    Editable = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsletterRecipients", x => x.NewsletterRecipientID);
                    table.ForeignKey(
                        name: "FK_NewsletterRecipients_NewsletterDistributionLists_NewsletterDistributionListID",
                        column: x => x.NewsletterDistributionListID,
                        principalTable: "NewsletterDistributionLists",
                        principalColumn: "NewsletterDistributionListID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NewsletterComponents_BelongsToNewsletterID",
                table: "NewsletterComponents",
                column: "BelongsToNewsletterID");

            migrationBuilder.CreateIndex(
                name: "IX_NewsletterRecipients_NewsletterDistributionListID",
                table: "NewsletterRecipients",
                column: "NewsletterDistributionListID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "NewsletterComponents");

            migrationBuilder.DropTable(
                name: "NewsletterComponentsTemplates");

            migrationBuilder.DropTable(
                name: "NewsletterLog");

            migrationBuilder.DropTable(
                name: "NewsletterRecipients");

            migrationBuilder.DropTable(
                name: "NewsletterSpool");

            migrationBuilder.DropTable(
                name: "NewsletterSpoolArchive");

            migrationBuilder.DropTable(
                name: "NewsletterTemplates");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Newsletter");

            migrationBuilder.DropTable(
                name: "NewsletterDistributionLists");
        }
    }
}
