using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleVault.Ef.Migrations
{
    /// <inheritdoc />
    public partial class fixPaymentmethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "MethodId",
                table: "Payments",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_MethodId",
                table: "Payments",
                column: "MethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PaymentMethods_MethodId",
                table: "Payments",
                column: "MethodId",
                principalTable: "PaymentMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentMethods_MethodId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_MethodId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "MethodId",
                table: "Payments");
        }
    }
}
