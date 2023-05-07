using Microsoft.EntityFrameworkCore.Migrations;
using Simulation.DatabaseBase.Base.Database;
using Simulation.DatabaseBase.Common.Helpers;
using Simulation.DatabaseBase.Common.Models.Core;

#nullable disable

namespace Simulation.DatabaseBase.Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class CreateAuthorTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: nameof(Author),
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                }
            );

            migrationBuilder.SetIndex(nameof(Author), "Status");
            migrationBuilder.SetIndex(
                table: nameof(Author),
                column: "CreatedAt",
                descending: new[] { true }
            );
            migrationBuilder.SetIndex(
                table: nameof(Author),
                column: new[] { "Name", "Age" },
                descending: new[] { true, false },
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(nameof(Author));
        }
    }
}