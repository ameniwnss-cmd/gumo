using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gumo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTrackingAndConfirmation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrackingToken",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ConfirmationCode",
                table: "EventBookings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "EventBookings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UsedAt",
                table: "EventBookings",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackingToken",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ConfirmationCode",
                table: "EventBookings");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "EventBookings");

            migrationBuilder.DropColumn(
                name: "UsedAt",
                table: "EventBookings");
        }
    }
}
