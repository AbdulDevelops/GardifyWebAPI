using Microsoft.EntityFrameworkCore.Migrations;

namespace FileUploadLib.Migrations
{
    public partial class addoldid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Modules",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "FileToModules",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Files",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "FileToModules");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Files");
        }
    }
}
