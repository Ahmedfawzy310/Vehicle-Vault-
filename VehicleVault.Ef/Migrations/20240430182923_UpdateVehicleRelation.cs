using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleVault.Ef.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVehicleRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "TypeId",
                table: "Vehicles",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_TypeId",
                table: "Vehicles",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleTypes_TypeId",
                table: "Vehicles",
                column: "TypeId",
                principalTable: "VehicleTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleTypes_TypeId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_TypeId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Vehicles");
        }
    }
}
