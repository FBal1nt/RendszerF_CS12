using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JegyMester.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class RoomChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Capacity",
                table: "Rooms",
                newName: "SeatsPerRow");

            migrationBuilder.AddColumn<int>(
                name: "RoomNumber",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RowCount",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomNumber",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "RowCount",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "SeatsPerRow",
                table: "Rooms",
                newName: "Capacity");
        }
    }
}
