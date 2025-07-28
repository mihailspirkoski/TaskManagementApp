using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 1, "admin@adming.com", "$2a$11$wfJjcaLUG69Uc5qJ9dNTi.eqAOCSANHoD0Ft7mqJvUKrwcuhoIIMS", 1 },
                    { 2, "user@user.com", "$2a$11$U9JNhqhNvl7bDxMRomV59ej5905Pg9lPpueIFLHU856sfhr.l105u", 2 }
                });

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "CompletedAt", "Description", "DueDate", "IsCompleted", "Title", "UserId" },
                values: new object[] { 1, null, "Set up", new DateTime(2025, 8, 4, 19, 51, 46, 406, DateTimeKind.Utc).AddTicks(4603), false, "Complete project setup", 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
