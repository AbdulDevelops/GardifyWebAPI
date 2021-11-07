using Microsoft.EntityFrameworkCore.Migrations;

namespace GardifyNewsletter.Migrations
{
    public partial class ChangedNewPlantFieldType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NewPlant3SubHeadline",
                table: "NewsletterNewPlants",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NewPlant2SubHeadline",
                table: "NewsletterNewPlants",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NewPlant1SubHeadline",
                table: "NewsletterNewPlants",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "NewPlant3SubHeadline",
                table: "NewsletterNewPlants",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NewPlant2SubHeadline",
                table: "NewsletterNewPlants",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NewPlant1SubHeadline",
                table: "NewsletterNewPlants",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
