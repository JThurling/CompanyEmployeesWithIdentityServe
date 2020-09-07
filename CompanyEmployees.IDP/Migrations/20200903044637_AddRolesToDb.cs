using Microsoft.EntityFrameworkCore.Migrations;

namespace CompanyEmployees.IDP.Migrations
{
    public partial class AddRolesToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "c3a0cb55-ddaf-4f2f-8419-f3f937698aa1", "ed72bf71-e34c-4892-ac5b-f9623829b530", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "6d506b42-9fa0-4ef7-a92a-0b5b0a123665", "908ec3c0-6e73-4d59-953f-285a4d08b6d8", "Visitor", "VISITOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6d506b42-9fa0-4ef7-a92a-0b5b0a123665");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c3a0cb55-ddaf-4f2f-8419-f3f937698aa1");
        }
    }
}
