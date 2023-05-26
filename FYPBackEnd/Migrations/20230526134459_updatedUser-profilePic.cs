using Microsoft.EntityFrameworkCore.Migrations;

namespace FYPBackEnd.Migrations
{
    public partial class updatedUserprofilePic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProficePictureId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProficePictureId",
                table: "AspNetUsers");
        }
    }
}
