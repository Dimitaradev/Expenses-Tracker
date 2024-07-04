using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Expences_Tracker.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumnDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MyProperty",
                table: "Transactions",
                newName: "Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Transactions",
                newName: "MyProperty");
        }
    }
}
