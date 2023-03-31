using Microsoft.EntityFrameworkCore.Migrations;

namespace FYPBackEnd.Migrations
{
    public partial class updateUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsKYCComplete",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsKYCComplete",
                table: "AspNetUsers");
        }
    }
}
