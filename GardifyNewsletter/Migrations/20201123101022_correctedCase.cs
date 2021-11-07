using Microsoft.EntityFrameworkCore.Migrations;

namespace GardifyNewsletter.Migrations
{
    public partial class correctedCase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "newPlantArticleId3",
                table: "NewsletterComponents",
                newName: "NewPlantArticleId3");

            migrationBuilder.RenameColumn(
                name: "newPlantArticleId2",
                table: "NewsletterComponents",
                newName: "NewPlantArticleId2");

            migrationBuilder.RenameColumn(
                name: "newPlantArticleId1",
                table: "NewsletterComponents",
                newName: "NewPlantArticleId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NewPlantArticleId3",
                table: "NewsletterComponents",
                newName: "newPlantArticleId3");

            migrationBuilder.RenameColumn(
                name: "NewPlantArticleId2",
                table: "NewsletterComponents",
                newName: "newPlantArticleId2");

            migrationBuilder.RenameColumn(
                name: "NewPlantArticleId1",
                table: "NewsletterComponents",
                newName: "newPlantArticleId1");
        }
    }
}
