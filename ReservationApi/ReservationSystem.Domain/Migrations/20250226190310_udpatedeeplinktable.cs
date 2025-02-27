using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationSystem.Domain.Migrations
{
    /// <inheritdoc />
    public partial class udpatedeeplinktable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "adults2",
                table: "deeplink",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "adults3",
                table: "deeplink",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cabin_class2",
                table: "deeplink",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cabin_class3",
                table: "deeplink",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "children2",
                table: "deeplink",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "children3",
                table: "deeplink",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "departuredate2",
                table: "deeplink",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "departuredate3",
                table: "deeplink",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "destination2",
                table: "deeplink",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "destination3",
                table: "deeplink",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "flight_type2",
                table: "deeplink",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "flight_type3",
                table: "deeplink",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "infant2",
                table: "deeplink",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "infant3",
                table: "deeplink",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "origin2",
                table: "deeplink",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "origin3",
                table: "deeplink",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "returndate2",
                table: "deeplink",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "returndate3",
                table: "deeplink",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "adults2",
                table: "deeplink");

            migrationBuilder.DropColumn(
                name: "adults3",
                table: "deeplink");

            migrationBuilder.DropColumn(
                name: "cabin_class2",
                table: "deeplink");

            migrationBuilder.DropColumn(
                name: "cabin_class3",
                table: "deeplink");

            migrationBuilder.DropColumn(
                name: "children2",
                table: "deeplink");

            migrationBuilder.DropColumn(
                name: "children3",
                table: "deeplink");

            migrationBuilder.DropColumn(
                name: "departuredate2",
                table: "deeplink");

            migrationBuilder.DropColumn(
                name: "departuredate3",
                table: "deeplink");

            migrationBuilder.DropColumn(
                name: "destination2",
                table: "deeplink");

            migrationBuilder.DropColumn(
                name: "destination3",
                table: "deeplink");

            migrationBuilder.DropColumn(
                name: "flight_type2",
                table: "deeplink");

            migrationBuilder.DropColumn(
                name: "flight_type3",
                table: "deeplink");

            migrationBuilder.DropColumn(
                name: "infant2",
                table: "deeplink");

            migrationBuilder.DropColumn(
                name: "infant3",
                table: "deeplink");

            migrationBuilder.DropColumn(
                name: "origin2",
                table: "deeplink");

            migrationBuilder.DropColumn(
                name: "origin3",
                table: "deeplink");

            migrationBuilder.DropColumn(
                name: "returndate2",
                table: "deeplink");

            migrationBuilder.DropColumn(
                name: "returndate3",
                table: "deeplink");
        }
    }
}
