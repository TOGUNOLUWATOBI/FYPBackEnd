using Microsoft.EntityFrameworkCore.Migrations;

namespace FYPBackEnd.Migrations
{
    public partial class updatedUserWithDeviceTokenAndDeviceType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BeneficiaryAccountNumber",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BeneficiaryBank",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAndroidDevice",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeneficiaryAccountNumber",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "BeneficiaryBank",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DeviceToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsAndroidDevice",
                table: "AspNetUsers");
        }
    }
}
