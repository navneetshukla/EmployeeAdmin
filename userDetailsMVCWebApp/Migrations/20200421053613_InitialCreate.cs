using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace userDetailsMVCWebApp.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeName = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    EmailId = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    DOB = table.Column<DateTime>(nullable: false),
                    Gender = table.Column<string>(type: "varchar(10)", nullable: true),
                    MarritalStatus = table.Column<string>(type: "varchar(20)", nullable: true),
                    Anniversary = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
