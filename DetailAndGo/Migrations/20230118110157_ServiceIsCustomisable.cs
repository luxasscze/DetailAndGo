using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DetailAndGo.Migrations
{
    public partial class ServiceIsCustomisable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCustomisable",
                table: "Services",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCustomisable",
                table: "Services");
        }
    }
}
