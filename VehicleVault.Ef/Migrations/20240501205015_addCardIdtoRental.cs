using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleVault.Ef.Migrations
{
    /// <inheritdoc />
    public partial class addCardIdtoRental : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackCard",
                table: "Rentals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FrontCard",
                table: "Rentals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackCard",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "FrontCard",
                table: "Rentals");
        }
    }
}
