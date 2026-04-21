using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JegyMester.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class GuestPurchaseSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketPurchases_Users_UserId",
                table: "TicketPurchases");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "TicketPurchases",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "GuestEmail",
                table: "TicketPurchases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuestPhone",
                table: "TicketPurchases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketPurchases_Users_UserId",
                table: "TicketPurchases",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketPurchases_Users_UserId",
                table: "TicketPurchases");

            migrationBuilder.DropColumn(
                name: "GuestEmail",
                table: "TicketPurchases");

            migrationBuilder.DropColumn(
                name: "GuestPhone",
                table: "TicketPurchases");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "TicketPurchases",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketPurchases_Users_UserId",
                table: "TicketPurchases",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
