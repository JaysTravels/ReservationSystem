using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ReservationSystem.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ActiveUsers2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "active_users",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    session_id = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    ip_address = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    last_active = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_active_users", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "active_users",
                schema: "public");
        }
    }
}
