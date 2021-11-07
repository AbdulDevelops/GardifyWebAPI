using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FileUploadLib.Migrations
{
    public partial class restructurefileclass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileOptions");

            migrationBuilder.RenameColumn(
                name: "ScaleWidth",
                table: "Files",
                newName: "Width");

            migrationBuilder.AddColumn<string>(
                name: "LinkTitle",
                table: "FileToModules",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkUrl",
                table: "FileToModules",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "FileToModules",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deletable",
                table: "Files",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Downloadable",
                table: "Files",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Editable",
                table: "Files",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "Files",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGaleryImage",
                table: "Files",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Published",
                table: "Files",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkTitle",
                table: "FileToModules");

            migrationBuilder.DropColumn(
                name: "LinkUrl",
                table: "FileToModules");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "FileToModules");

            migrationBuilder.DropColumn(
                name: "Deletable",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Downloadable",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Editable",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "IsGaleryImage",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Published",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "Width",
                table: "Files",
                newName: "ScaleWidth");

            migrationBuilder.CreateTable(
                name: "FileOptions",
                columns: table => new
                {
                    FileOptionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Deletable = table.Column<bool>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    Downloadable = table.Column<bool>(nullable: false),
                    Editable = table.Column<bool>(nullable: false),
                    EditedBy = table.Column<string>(maxLength: 128, nullable: true),
                    EditedDate = table.Column<DateTime>(nullable: false),
                    FileId = table.Column<int>(nullable: false),
                    IsGaleryImage = table.Column<bool>(nullable: false),
                    Published = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileOptions", x => x.FileOptionId);
                    table.ForeignKey(
                        name: "FK_FileOptions_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "FileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileOptions_FileId",
                table: "FileOptions",
                column: "FileId");
        }
    }
}
