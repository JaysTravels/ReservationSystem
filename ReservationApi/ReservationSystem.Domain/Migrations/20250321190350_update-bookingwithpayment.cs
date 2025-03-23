using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationSystem.Domain.Migrations
{
    /// <inheritdoc />
    public partial class updatebookingwithpayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ip",
                table: "booking_info",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "acceptance",
                table: "booking_info",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "authorization_code",
                table: "booking_info",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "barclays_status",
                table: "booking_info",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "brand",
                table: "booking_info",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "card_holder_name",
                table: "booking_info",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "card_number",
                table: "booking_info",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "expiry_date",
                table: "booking_info",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "order_id",
                table: "booking_info",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payment_method",
                table: "booking_info",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ip",
                table: "booking_info");

            migrationBuilder.DropColumn(
                name: "acceptance",
                table: "booking_info");

            migrationBuilder.DropColumn(
                name: "authorization_code",
                table: "booking_info");

            migrationBuilder.DropColumn(
                name: "barclays_status",
                table: "booking_info");

            migrationBuilder.DropColumn(
                name: "brand",
                table: "booking_info");

            migrationBuilder.DropColumn(
                name: "card_holder_name",
                table: "booking_info");

            migrationBuilder.DropColumn(
                name: "card_number",
                table: "booking_info");

            migrationBuilder.DropColumn(
                name: "expiry_date",
                table: "booking_info");

            migrationBuilder.DropColumn(
                name: "order_id",
                table: "booking_info");

            migrationBuilder.DropColumn(
                name: "payment_method",
                table: "booking_info");
        }
    }
}
