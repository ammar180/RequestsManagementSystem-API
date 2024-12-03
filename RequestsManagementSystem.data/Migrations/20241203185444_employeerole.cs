using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RequestsManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class employeerole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeRole",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeRole",
                table: "Employees");
        }
    }
}
