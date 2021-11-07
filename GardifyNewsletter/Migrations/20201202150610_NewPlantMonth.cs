using Microsoft.EntityFrameworkCore.Migrations;

namespace GardifyNewsletter.Migrations
{
    public partial class NewPlantMonth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewPlantMonth",
                table: "NewsletterComponents",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewPlantMonth",
                table: "NewsletterComponents");
        }
    }
}
