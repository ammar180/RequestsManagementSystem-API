using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RequestsManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class parentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Transactions",
                newName: "TypeId");

            migrationBuilder.AddColumn<string>(
                name: "ParentType",
                table: "TransactionTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "TransactionTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "ParentType",
                value: "");

            migrationBuilder.UpdateData(
                table: "TransactionTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "ParentType",
                value: "");

            migrationBuilder.UpdateData(
                table: "TransactionTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "ParentType",
                value: "RegularLeave");

            migrationBuilder.UpdateData(
                table: "TransactionTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "ParentType",
                value: "RegularLeave");

            migrationBuilder.UpdateData(
                table: "TransactionTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "ParentType",
                value: "RegularLeave");

            migrationBuilder.UpdateData(
                table: "TransactionTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "ParentType",
                value: "CasualLeave");

            migrationBuilder.UpdateData(
                table: "TransactionTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "ParentType",
                value: "");

            migrationBuilder.UpdateData(
                table: "TransactionTypes",
                keyColumn: "Id",
                keyValue: 8,
                column: "ParentType",
                value: "");

            migrationBuilder.UpdateData(
                table: "TransactionTypes",
                keyColumn: "Id",
                keyValue: 9,
                column: "ParentType",
                value: "");

            migrationBuilder.UpdateData(
                table: "TransactionTypes",
                keyColumn: "Id",
                keyValue: 10,
                column: "ParentType",
                value: "");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TypeId",
                table: "Transactions",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_TransactionTypes_TypeId",
                table: "Transactions",
                column: "TypeId",
                principalTable: "TransactionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_TransactionTypes_TypeId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TypeId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ParentType",
                table: "TransactionTypes");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                table: "Transactions",
                newName: "Type");

            migrationBuilder.AddColumn<short>(
                name: "Title",
                table: "Transactions",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }
    }
}
