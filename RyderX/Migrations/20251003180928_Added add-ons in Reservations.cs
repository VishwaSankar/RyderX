using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RyderX_Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedaddonsinReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AdditionalDriver",
                table: "Reservations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "AdditionalDriverFee",
                table: "Reservations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "ChildSeat",
                table: "Reservations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ChildSeatFee",
                table: "Reservations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "RoadCare",
                table: "Reservations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "RoadCareFee",
                table: "Reservations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalDriver",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "AdditionalDriverFee",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ChildSeat",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ChildSeatFee",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "RoadCare",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "RoadCareFee",
                table: "Reservations");
        }
    }
}
