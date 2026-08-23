using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sekka.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCoordinatesToRide : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DistanceInKm",
                table: "Rides",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DropoffLat",
                table: "Rides",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DropoffLng",
                table: "Rides",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PickupLat",
                table: "Rides",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PickupLng",
                table: "Rides",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistanceInKm",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "DropoffLat",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "DropoffLng",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "PickupLat",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "PickupLng",
                table: "Rides");
        }
    }
}
