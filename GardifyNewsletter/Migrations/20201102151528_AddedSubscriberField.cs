using Microsoft.EntityFrameworkCore.Migrations;

namespace GardifyNewsletter.Migrations
{
    public partial class AddedSubscriberField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "NewsletterSpool");

            migrationBuilder.AddColumn<bool>(
                name: "Subscriber",
                table: "NewsletterSpool",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subscriber",
                table: "NewsletterSpool");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "NewsletterSpool",
                nullable: true);
        }
    }
}
