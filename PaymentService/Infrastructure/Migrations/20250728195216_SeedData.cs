using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Subscriptions",
                columns: new[] { "Id", "CreatedAt", "Status", "StripeSubscriptionId", "UpdatedAt", "UserId" },
                values: new object[] { 1, new DateTime(2025, 7, 28, 19, 52, 14, 904, DateTimeKind.Utc).AddTicks(991), "Active", "sub_123456789", null, 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
