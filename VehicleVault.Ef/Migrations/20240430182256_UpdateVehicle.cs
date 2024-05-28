using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleVault.Ef.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleTypes_TypeId1",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_TypeId1",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "TypeId1",
                table: "Vehicles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "Vehicles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "TypeId1",
                table: "Vehicles",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_TypeId1",
                table: "Vehicles",
                column: "TypeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleTypes_TypeId1",
                table: "Vehicles",
                column: "TypeId1",
                principalTable: "VehicleTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
