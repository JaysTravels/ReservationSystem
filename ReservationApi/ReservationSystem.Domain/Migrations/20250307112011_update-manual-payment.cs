using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationSystem.Domain.Migrations
{
    /// <inheritdoc />
    public partial class updatemanualpayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ip",
                table: "manual_payment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "acceptance",
                table: "manual_payment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "barclays_status",
                table: "manual_payment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "brand",
                table: "manual_payment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "card_holder_name",
                table: "manual_payment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "card_number",
                table: "manual_payment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "error",
                table: "manual_payment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "expiry_date",
                table: "manual_payment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "order_id",
                table: "manual_payment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payment_method",
                table: "manual_payment",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ip",
                table: "manual_payment");

            migrationBuilder.DropColumn(
                name: "acceptance",
                table: "manual_payment");

            migrationBuilder.DropColumn(
                name: "barclays_status",
                table: "manual_payment");

            migrationBuilder.DropColumn(
                name: "brand",
                table: "manual_payment");

            migrationBuilder.DropColumn(
                name: "card_holder_name",
                table: "manual_payment");

            migrationBuilder.DropColumn(
                name: "card_number",
                table: "manual_payment");

            migrationBuilder.DropColumn(
                name: "error",
                table: "manual_payment");

            migrationBuilder.DropColumn(
                name: "expiry_date",
                table: "manual_payment");

            migrationBuilder.DropColumn(
                name: "order_id",
                table: "manual_payment");

            migrationBuilder.DropColumn(
                name: "payment_method",
                table: "manual_payment");
        }
    }
}
