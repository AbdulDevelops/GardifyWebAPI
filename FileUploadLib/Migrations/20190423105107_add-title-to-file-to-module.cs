using Microsoft.EntityFrameworkCore.Migrations;

namespace FileUploadLib.Migrations
{
    public partial class addtitletofiletomodule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "FileToModules",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "FileToModules");
        }
    }
}
