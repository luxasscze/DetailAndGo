using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DetailAndGo.Migrations
{
    public partial class CarPrices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price4x4",
                table: "Services",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceLarge",
                table: "Services",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceMedium",
                table: "Services",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price4x4",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "PriceLarge",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "PriceMedium",
                table: "Services");
        }
    }
}
