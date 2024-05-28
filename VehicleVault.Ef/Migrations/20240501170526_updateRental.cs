using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleVault.Ef.Migrations
{
    /// <inheritdoc />
    public partial class updateRental : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Rentals");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Rentals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Rentals",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Rentals");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Rentals",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
