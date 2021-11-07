using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GardifyNewsletter.Migrations
{
    public partial class AddedNewPlantFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsletterSubComponents");

            migrationBuilder.AddColumn<int>(
                name: "newPlantArticleId1",
                table: "NewsletterComponents",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "newPlantArticleId2",
                table: "NewsletterComponents",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "newPlantArticleId3",
                table: "NewsletterComponents",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "newPlantArticleId1",
                table: "NewsletterComponents");

            migrationBuilder.DropColumn(
                name: "newPlantArticleId2",
                table: "NewsletterComponents");

            migrationBuilder.DropColumn(
                name: "newPlantArticleId3",
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
    }
}
