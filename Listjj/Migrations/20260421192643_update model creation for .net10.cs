using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Listjj.Migrations
{
    /// <inheritdoc />
    public partial class updatemodelcreationfornet10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("3ebb187e-89ca-4a50-b7ea-c845f9273242"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("6bfb7ea7-81c2-4f11-baab-e1d688ac5824"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), "a1b2c3d4-e5f6-7890-abcd-ef1234567111", "User", "USER" },
                    { new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567891"), "a1b2c3d4-e5f6-7890-abcd-ef1234567222", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567891"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("3ebb187e-89ca-4a50-b7ea-c845f9273242"), "8b3bc0e7-f3b3-452e-a983-dd5c9273ab08", "Admin", "ADMIN" },
                    { new Guid("6bfb7ea7-81c2-4f11-baab-e1d688ac5824"), "c0c84e3b-c327-4d17-bdef-72b9855736f6", "User", "USER" }
                });
        }
    }
}
