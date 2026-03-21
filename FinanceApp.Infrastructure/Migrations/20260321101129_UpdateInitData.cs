using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInitData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d2577930-3235-4516-9423-6236985880a7"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Favorites", "Name", "Password" },
                values: new object[] { new Guid("ca45f9c9-056f-469a-b7e5-628270abc068"), new List<string>(), "admin", "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Name",
                table: "Users",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Name",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ca45f9c9-056f-469a-b7e5-628270abc068"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Favorites", "Name", "Password" },
                values: new object[] { new Guid("d2577930-3235-4516-9423-6236985880a7"), new List<string>(), "admin", "admin" });
        }
    }
}
