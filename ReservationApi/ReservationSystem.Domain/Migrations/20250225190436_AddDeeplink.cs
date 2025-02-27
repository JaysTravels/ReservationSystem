using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ReservationSystem.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddDeeplink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "deeplink",
                columns: table => new
                {
                    deeplink_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    image_url = table.Column<string>(type: "text", nullable: true),
                    country_name = table.Column<string>(type: "text", nullable: true),
                    city_name1 = table.Column<string>(type: "text", nullable: true),
                    price1 = table.Column<decimal>(type: "numeric", nullable: true),
                    city_name2 = table.Column<string>(type: "text", nullable: true),
                    price2 = table.Column<decimal>(type: "numeric", nullable: true),
                    city_name3 = table.Column<string>(type: "text", nullable: true),
                    price3 = table.Column<decimal>(type: "numeric", nullable: true),
                    adults = table.Column<int>(type: "integer", nullable: true),
                    children = table.Column<int>(type: "integer", nullable: true),
                    infant = table.Column<int>(type: "integer", nullable: true),
                    departuredate = table.Column<string>(type: "text", nullable: true),
                    returndate = table.Column<string>(type: "text", nullable: true),
                    origin = table.Column<string>(type: "text", nullable: true),
                    destination = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true),
                    created_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deeplink", x => x.deeplink_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "deeplink");
        }
    }
}
