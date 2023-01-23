using Microsoft.EntityFrameworkCore.Migrations;

namespace CRUD1.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deatils",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "users",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Details",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "Deatils",
                table: "users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
