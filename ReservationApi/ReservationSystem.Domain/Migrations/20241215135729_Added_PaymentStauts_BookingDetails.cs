﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationSystem.Domain.Migrations
{
    /// <inheritdoc />
    public partial class Added_PaymentStauts_BookingDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "booking_info",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "error",
                table: "booking_info",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payment_status",
                table: "booking_info",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_on",
                table: "booking_info");

            migrationBuilder.DropColumn(
                name: "error",
                table: "booking_info");

            migrationBuilder.DropColumn(
                name: "payment_status",
                table: "booking_info");
        }
    }
}