using Microsoft.EntityFrameworkCore.Migrations;

namespace FYPBackEnd.Migrations
{
    public partial class update_user_ispanicpinset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPanicPINSet",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPanicPINSet",
                table: "AspNetUsers");
        }
    }
}
