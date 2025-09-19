using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCms.Api.Migrations
{
    /// <inheritdoc />
    public partial class MembersTableExists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Members table already exists, so this is an empty migration
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Members");
        }
    }
}
