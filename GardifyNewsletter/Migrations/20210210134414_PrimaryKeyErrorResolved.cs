using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GardifyNewsletter.Migrations
{
    public partial class PrimaryKeyErrorResolved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NewsletterNewPlants",
                table: "NewsletterNewPlants");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "NewsletterNewPlants",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NewsletterNewPlants",
                table: "NewsletterNewPlants",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_NewsletterNewPlants_NewPlantComponentId",
                table: "NewsletterNewPlants",
                column: "NewPlantComponentId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NewsletterNewPlants",
                table: "NewsletterNewPlants");

            migrationBuilder.DropIndex(
                name: "IX_NewsletterNewPlants_NewPlantComponentId",
                table: "NewsletterNewPlants");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "NewsletterNewPlants");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NewsletterNewPlants",
                table: "NewsletterNewPlants",
                column: "NewPlantComponentId");
        }
    }
}
