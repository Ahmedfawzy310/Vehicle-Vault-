using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleVault.Ef.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"INSERT INTO [dbo].[AspNetRoles] VALUES ('{Guid.NewGuid()}', 'User', 'USER', '{Guid.NewGuid()}')");
            migrationBuilder.Sql($"INSERT INTO [dbo].[AspNetRoles] VALUES ('{Guid.NewGuid()}', 'Admin', 'ADMIN', '{Guid.NewGuid()}')");
            migrationBuilder.Sql($"INSERT INTO [dbo].[AspNetRoles] VALUES ('{Guid.NewGuid()}', 'Manger', 'MANGER', '{Guid.NewGuid()}')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DELETE FROM [dbo].[AspNetRoles] WHERE Name = 'User'");
            migrationBuilder.Sql($"DELETE FROM [dbo].[AspNetRoles] WHERE Name = 'Admin'");
            migrationBuilder.Sql($"DELETE FROM [dbo].[AspNetRoles] WHERE Name = 'Manger'");
        }
    }
}
