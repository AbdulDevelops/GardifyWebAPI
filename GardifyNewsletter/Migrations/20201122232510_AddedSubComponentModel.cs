using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GardifyNewsletter.Migrations
{
    public partial class AddedSubComponentModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewPlantArticleId",
                table: "NewsletterComponents");

            migrationBuilder.CreateTable(
                name: "NewsletterSubComponents",
                columns: table => new
                {
                    NewsletterSubComponentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NewPlantArticleId = table.Column<int>(nullable: true),
                    NewsletterComponentTemplateId = table.Column<int>(nullable: true),
                    newsletterComponentsNewsletterComponentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsletterSubComponents", x => x.NewsletterSubComponentId);
                    table.ForeignKey(
                        name: "FK_NewsletterSubComponents_NewsletterComponents_newsletterComponentsNewsletterComponentId",
                        column: x => x.newsletterComponentsNewsletterComponentId,
                        principalTable: "NewsletterComponents",
                        principalColumn: "NewsletterComponentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NewsletterSubComponents_newsletterComponentsNewsletterComponentId",
                table: "NewsletterSubComponents",
                column: "newsletterComponentsNewsletterComponentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsletterSubComponents");

            migrationBuilder.AddColumn<int>(
                name: "NewPlantArticleId",
                table: "NewsletterComponents",
                nullable: true);
        }
    }
}
