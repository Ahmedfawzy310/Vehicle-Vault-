using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleVault.Ef.Migrations
{
    /// <inheritdoc />
    public partial class someUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAvaiable",
                table: "Vehicles",
                newName: "IsAvailable");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Rentals",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Rentals");

            migrationBuilder.RenameColumn(
                name: "IsAvailable",
                table: "Vehicles",
                newName: "IsAvaiable");
        }
    }
}
