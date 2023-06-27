using Microsoft.EntityFrameworkCore.Migrations;

namespace FYPBackEnd.Migrations
{
    public partial class lastday : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HasPaniced",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasPaniced",
                table: "AspNetUsers");
        }
    }
}
