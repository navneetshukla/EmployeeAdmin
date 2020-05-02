using Microsoft.EntityFrameworkCore.Migrations;

namespace userDetailsMVCWebApp.Migrations
{
    public partial class newcolumnFileName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "fileName",
                table: "Employees",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fileName",
                table: "Employees");
        }
    }
}
