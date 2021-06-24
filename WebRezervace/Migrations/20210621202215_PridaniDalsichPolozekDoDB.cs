using Microsoft.EntityFrameworkCore.Migrations;

namespace WebRezervace.Migrations
{
    public partial class PridaniDalsichPolozekDoDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Tel",
                table: "Uzivatele",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "AdminOpravneni",
                table: "Uzivatele",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "JmenoAPrijmeni",
                table: "Rezervace",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminOpravneni",
                table: "Uzivatele");

            migrationBuilder.DropColumn(
                name: "JmenoAPrijmeni",
                table: "Rezervace");

            migrationBuilder.AlterColumn<string>(
                name: "Tel",
                table: "Uzivatele",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
