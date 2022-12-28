using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DetailAndGo.Migrations
{
    public partial class CarSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CarSize",
                table: "Cars",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarSize",
                table: "Cars");
        }
    }
}
