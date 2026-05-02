using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JegyMester.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class TicketCalcellationReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Screenings_ScreeningId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_ScreeningId_Row_SeatNumber",
                table: "Tickets");

            migrationBuilder.AlterColumn<int>(
                name: "ScreeningId",
                table: "Tickets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ScreeningId_Row_SeatNumber",
                table: "Tickets",
                columns: new[] { "ScreeningId", "Row", "SeatNumber" },
                unique: true,
                filter: "[ScreeningId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Screenings_ScreeningId",
                table: "Tickets",
                column: "ScreeningId",
                principalTable: "Screenings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Screenings_ScreeningId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_ScreeningId_Row_SeatNumber",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Tickets");

            migrationBuilder.AlterColumn<int>(
                name: "ScreeningId",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ScreeningId_Row_SeatNumber",
                table: "Tickets",
                columns: new[] { "ScreeningId", "Row", "SeatNumber" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Screenings_ScreeningId",
                table: "Tickets",
                column: "ScreeningId",
                principalTable: "Screenings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
