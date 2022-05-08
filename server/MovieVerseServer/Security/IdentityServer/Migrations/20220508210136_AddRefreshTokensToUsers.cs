using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityServer.Migrations
{
    public partial class AddRefreshTokensToUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5acf127a-c863-41d0-9ac9-bbc3f1e18274");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "92f11aeb-8df5-46a0-b99b-0b7225ca973a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "db314346-682b-47ea-9ef9-f9cb70204560");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "12e62be5-9d63-440e-87ef-30f8b133b225", "915bbd20-03b1-43f4-b08b-345047c8fded", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "739c93c6-75ee-4f41-a0ee-4db20e981abf", "f422271f-b2d3-4555-a628-2acb1338b137", "BusinessUser", "BUSINESSUSER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "6d378b29-e68f-426b-b2e9-f7842134586b", "378e27a9-633a-4852-bb63-8316444ccc63", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "12e62be5-9d63-440e-87ef-30f8b133b225");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6d378b29-e68f-426b-b2e9-f7842134586b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "739c93c6-75ee-4f41-a0ee-4db20e981abf");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "92f11aeb-8df5-46a0-b99b-0b7225ca973a", "8014992f-00a8-4a9c-a207-9fb03fde4c7a", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "5acf127a-c863-41d0-9ac9-bbc3f1e18274", "0481527d-c442-4e5d-af83-e6309b75c5e3", "BusinessUser", "BUSINESSUSER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "db314346-682b-47ea-9ef9-f9cb70204560", "1cff48ad-2d5c-4a7c-bb6f-8ebec9875aa2", "Administrator", "ADMINISTRATOR" });
        }
    }
}
