using Microsoft.EntityFrameworkCore.Migrations;

namespace WebRezervace.Migrations
{
    public partial class PridaniMoznostiEmaluATelDoRezervace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ZpravaProAdmina",
                table: "Rezervace",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ZpravaProAdmina",
                table: "Rezervace");
        }
    }
}
