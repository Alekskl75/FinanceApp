using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInitData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Favorites", "Name", "Password" },
                values: new object[] { new Guid("d2577930-3235-4516-9423-6236985880a7"), new List<string>(), "admin", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d2577930-3235-4516-9423-6236985880a7"));
        }
    }
}
