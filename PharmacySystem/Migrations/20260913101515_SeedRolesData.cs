using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PharmacySystem.Migrations
{
    /// <inheritdoc />
    public partial class SeedRolesData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a182b8a0-2f22-49f3-8b1e-0d12e3456781", "ac9965c6-60b5-481a-adfe-b0df2561251b", "Admin", "ADMIN" },
                    { "b273c9b1-3f33-40a4-9c2f-1e23f4567892", "8d769b15-2f7b-44c2-bae7-b8009de9565f", "Pharmacist", "PHARMACIST" },
                    { "c384d0c2-4f44-51b5-ad30-2f34a5678903", "fb37521b-7185-4103-840e-11a0163bea49", "Cashier", "CASHIER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a182b8a0-2f22-49f3-8b1e-0d12e3456781");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b273c9b1-3f33-40a4-9c2f-1e23f4567892");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c384d0c2-4f44-51b5-ad30-2f34a5678903");
        }
    }
}
