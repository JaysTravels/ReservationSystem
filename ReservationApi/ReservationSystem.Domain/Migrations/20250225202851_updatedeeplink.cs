using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationSystem.Domain.Migrations
{
    /// <inheritdoc />
    public partial class updatedeeplink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cabin_class",
                table: "deeplink",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "flight_type",
                table: "deeplink",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cabin_class",
                table: "deeplink");

            migrationBuilder.DropColumn(
                name: "flight_type",
                table: "deeplink");
        }
    }
}
