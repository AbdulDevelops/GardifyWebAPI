using Microsoft.EntityFrameworkCore.Migrations;

namespace GardifyNewsletter.Migrations
{
    public partial class AddedNewPLantSubHeadline : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewPlantArticleId1",
                table: "NewsletterComponents");

            migrationBuilder.DropColumn(
                name: "NewPlantArticleId2",
                table: "NewsletterComponents");

            migrationBuilder.DropColumn(
                name: "NewPlantArticleId3",
                table: "NewsletterComponents");

            migrationBuilder.DropColumn(
                name: "NewPlantMonth",
                table: "NewsletterComponents");

            migrationBuilder.CreateTable(
                name: "NewsletterNewPlants",
                columns: table => new
                {
                    NewPlantComponentId = table.Column<int>(nullable: false),
                    NewPlant1 = table.Column<int>(nullable: true),
                    NewPlant1SubHeadline = table.Column<int>(nullable: true),
                    NewPlant2 = table.Column<int>(nullable: true),
                    NewPlant2SubHeadline = table.Column<int>(nullable: true),
                    NewPlant3 = table.Column<int>(nullable: true),
                    NewPlant3SubHeadline = table.Column<int>(nullable: true),
                    NewPlantMonth = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsletterNewPlants", x => x.NewPlantComponentId);
                    table.ForeignKey(
                        name: "FK_NewsletterNewPlants_NewsletterComponents_NewPlantComponentId",
                        column: x => x.NewPlantComponentId,
                        principalTable: "NewsletterComponents",
                        principalColumn: "NewsletterComponentID",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsletterNewPlants");

            migrationBuilder.AddColumn<int>(
                name: "NewPlantArticleId1",
                table: "NewsletterComponents",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NewPlantArticleId2",
                table: "NewsletterComponents",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NewPlantArticleId3",
                table: "NewsletterComponents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewPlantMonth",
                table: "NewsletterComponents",
                nullable: true);
        }
    }
}
