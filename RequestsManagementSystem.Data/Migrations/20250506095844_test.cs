using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RequestsManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeLevel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LevelName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LevelDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegularLeaveperYear = table.Column<int>(type: "int", nullable: false),
                    CasualLeavePerYear = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeLevel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Unit = table.Column<double>(type: "float", nullable: false),
                    Sign = table.Column<int>(type: "int", nullable: false),
                    ParentType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DateOfEmployment = table.Column<DateOnly>(type: "date", nullable: false),
                    EmployeeRole = table.Column<short>(type: "smallint", nullable: false),
                    EmployeeLevelId = table.Column<int>(type: "int", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ManagerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_EmployeeLevel_EmployeeLevelId",
                        column: x => x.EmployeeLevelId,
                        principalTable: "EmployeeLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_Employees_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubstituteEmployeeId = table.Column<int>(type: "int", nullable: true),
                    Itinerary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RespondDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RespondMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    SeenStatus = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Employees_SubstituteEmployeeId",
                        column: x => x.SubstituteEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_TransactionTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "TransactionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "EmployeeLevel",
                columns: new[] { "Id", "CasualLeavePerYear", "LevelDescription", "LevelName", "OrderId", "RegularLeaveperYear" },
                values: new object[,]
                {
                    { 1, 6, "الفئة أ", "A", 1, 15 },
                    { 2, 6, "الفئة ب", "B", 2, 24 }
                });

            migrationBuilder.InsertData(
                table: "TransactionTypes",
                columns: new[] { "Id", "Description", "Name", "ParentType", "Sign", "Unit" },
                values: new object[,]
                {
                    { 1, "إجازة عارضه", "CasualLeave", "", -1, 1.0 },
                    { 2, "إجازة اعتيادية", "RegularLeave", "", -1, 1.0 },
                    { 3, "نصف يوم", "HalfDay", "RegularLeave", -1, 0.5 },
                    { 4, "ربع يوم", "QuarterDay", "RegularLeave", -1, 0.25 },
                    { 5, "رصيد اعتيادي إضافي", "AdditionalRegularLeave", "RegularLeave", 1, 1.0 },
                    { 6, "رصيد عارضه إضافي", "AdditionalCasualLeave", "CasualLeave", 1, 1.0 },
                    { 7, "غياب بأذن", "ExcusedAbsent", "", -1, 1.0 },
                    { 8, "غياب بدون بأذن", "UnexcusedAbsent", "", -1, 1.0 },
                    { 9, "يوم كامل", "FullDay", "", 0, 1.0 },
                    { 10, "يوم جزئي", "PartialDay", "", 0, 0.5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Code",
                table: "Employees",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeLevelId",
                table: "Employees",
                column: "EmployeeLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ManagerId",
                table: "Employees",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_EmployeeId",
                table: "Transactions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SubstituteEmployeeId",
                table: "Transactions",
                column: "SubstituteEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TypeId",
                table: "Transactions",
                column: "TypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "TransactionTypes");

            migrationBuilder.DropTable(
                name: "EmployeeLevel");
        }
    }
}
