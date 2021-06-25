using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebRezervace.Migrations
{
    public partial class VytvoreniDatabze : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Uzivatele",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Heslo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Jmeno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Prijmeni = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tel = table.Column<int>(type: "int", nullable: false),
                    AdminOpravneni = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uzivatele", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Rezervace",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Jmeno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prijmeni = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PocetOsob = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Doba = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tel = table.Column<int>(type: "int", nullable: false),
                    ZpravaProAdmina = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AutorID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezervace", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Rezervace_Uzivatele_AutorID",
                        column: x => x.AutorID,
                        principalTable: "Uzivatele",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rezervace_AutorID",
                table: "Rezervace",
                column: "AutorID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rezervace");

            migrationBuilder.DropTable(
                name: "Uzivatele");
        }
    }
}
