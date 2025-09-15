using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCms.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddWebsiteFeaturesOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Company = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Subject = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Reply = table.Column<string>(type: "TEXT", nullable: true),
                    RepliedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RepliedById = table.Column<int>(type: "INTEGER", nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    Excerpt = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    FeaturedImage = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Template = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    ShowInMenu = table.Column<bool>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: true),
                    SeoTitle = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SeoDescription = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    SeoKeywords = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedById = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedById = table.Column<int>(type: "INTEGER", nullable: true),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pages_Pages_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SeoSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PagePath = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    Keywords = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    OgTitle = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    OgDescription = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    OgImage = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    StructuredData = table.Column<string>(type: "TEXT", nullable: true),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeoSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebsiteConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Group = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DataType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteConfigs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pages_ParentId",
                table: "Pages",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_Slug",
                table: "Pages",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeoSettings_PagePath",
                table: "SeoSettings",
                column: "PagePath",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteConfigs_Key",
                table: "WebsiteConfigs",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Pages");

            migrationBuilder.DropTable(
                name: "SeoSettings");

            migrationBuilder.DropTable(
                name: "WebsiteConfigs");
        }
    }
}
