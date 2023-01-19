using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DetailAndGo.Migrations
{
    public partial class ServiceTimeToFinish2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeToFinishMins",
                table: "Services",
                newName: "TimeToFinishMinsS");

            migrationBuilder.AddColumn<int>(
                name: "TimeToFinishMinsL",
                table: "Services",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimeToFinishMinsM",
                table: "Services",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeToFinishMinsL",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "TimeToFinishMinsM",
                table: "Services");

            migrationBuilder.RenameColumn(
                name: "TimeToFinishMinsS",
                table: "Services",
                newName: "TimeToFinishMins");
        }
    }
}
