using Microsoft.EntityFrameworkCore.Migrations;

namespace FileUploadLib.Migrations
{
    public partial class addparentrelationtofiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentFileId",
                table: "Files",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScaleWidth",
                table: "Files",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_ParentFileId",
                table: "Files",
                column: "ParentFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Files_ParentFileId",
                table: "Files",
                column: "ParentFileId",
                principalTable: "Files",
                principalColumn: "FileId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Files_ParentFileId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_ParentFileId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ParentFileId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ScaleWidth",
                table: "Files");
        }
    }
}
