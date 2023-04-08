using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simulation.DatabaseBase.Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class Author : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Author",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                },
                constraints: table => { table.PrimaryKey("PrimaryKeyAuthor", x => x.Id); }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}