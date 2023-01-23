using Microsoft.EntityFrameworkCore.Migrations;

namespace CRUD1.Migrations
{
    public partial class changeLoginRule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "loginmodel",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "loginmodel");
        }
    }
}
