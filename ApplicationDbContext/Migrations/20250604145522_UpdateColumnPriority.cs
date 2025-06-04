using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class UpdateColumnPriority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "UserTasks",
                newName: "PriorityType");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "ac45f73f-d054-452b-8382-4c1063d45749");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "94aa3186-848e-45ad-a2e6-792c427b12b6");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f2cabb81-388e-46be-a34f-89c5f3101209", "AQAAAAEAACcQAAAAEAuZhAcD4hKDOZqNfbf6N2IgDIms8SFfreFRMqdDT04UTG118Od08dhOzQXZfk17oQ==", "6c116512-1df8-4c47-8df6-1b30d8b3bcba" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PriorityType",
                table: "UserTasks",
                newName: "Priority");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "f4f1c448-0b4b-4193-a95e-bc3975fde07a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "86fbdae5-638f-49da-bc5d-7b37eb9fd706");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ec8197be-0d2f-4c2d-a311-562290cb2835", "AQAAAAEAACcQAAAAEJK31pziMfMG/bKXWQlGpxMH4CDO3rm6z9XQcoQvB+do+kZ/0DiI+VK9doDWuKSYow==", "ec7d0b2d-9db9-4e6a-b778-b1afb8532e94" });
        }
    }
}
