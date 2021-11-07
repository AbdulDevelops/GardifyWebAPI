using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FileUploadLib.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    ApplicationId = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 128, nullable: true),
                    EditedDate = table.Column<DateTime>(nullable: false),
                    EditedBy = table.Column<string>(maxLength: 128, nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    RootPath = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.ApplicationId);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    FileId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 128, nullable: true),
                    EditedDate = table.Column<DateTime>(nullable: false),
                    EditedBy = table.Column<string>(maxLength: 128, nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    ApplicationId = table.Column<Guid>(nullable: false),
                    Path = table.Column<string>(nullable: false),
                    OriginalFileName = table.Column<string>(nullable: false),
                    NormalizedOriginalFileName = table.Column<string>(nullable: false),
                    Guid = table.Column<Guid>(nullable: false),
                    IsTestFile = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.FileId);
                    table.ForeignKey(
                        name: "FK_Files_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    ModuleId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 128, nullable: true),
                    EditedDate = table.Column<DateTime>(nullable: false),
                    EditedBy = table.Column<string>(maxLength: 128, nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    ApplicationId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.ModuleId);
                    table.ForeignKey(
                        name: "FK_Modules_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileOptions",
                columns: table => new
                {
                    FileOptionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 128, nullable: true),
                    EditedDate = table.Column<DateTime>(nullable: false),
                    EditedBy = table.Column<string>(maxLength: 128, nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    FileId = table.Column<int>(nullable: false),
                    Published = table.Column<bool>(nullable: false),
                    IsGaleryImage = table.Column<bool>(nullable: false),
                    Downloadable = table.Column<bool>(nullable: false),
                    Editable = table.Column<bool>(nullable: false),
                    Deletable = table.Column<bool>(nullable: false)
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

            migrationBuilder.CreateTable(
                name: "FileToModules",
                columns: table => new
                {
                    FileToModuleId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 128, nullable: true),
                    EditedDate = table.Column<DateTime>(nullable: false),
                    EditedBy = table.Column<string>(maxLength: 128, nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    FileId = table.Column<int>(nullable: false),
                    ModuleId = table.Column<int>(nullable: false),
                    DetailId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    AltText = table.Column<string>(nullable: true),
                    Sort = table.Column<int>(nullable: true),
                    IsTestFile = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileToModules", x => x.FileToModuleId);
                    table.ForeignKey(
                        name: "FK_FileToModules_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "FileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileToModules_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "ModuleId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileOptions_FileId",
                table: "FileOptions",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_ApplicationId",
                table: "Files",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_FileToModules_FileId",
                table: "FileToModules",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_FileToModules_ModuleId",
                table: "FileToModules",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Modules_ApplicationId",
                table: "Modules",
                column: "ApplicationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileOptions");

            migrationBuilder.DropTable(
                name: "FileToModules");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "Applications");
        }
    }
}
