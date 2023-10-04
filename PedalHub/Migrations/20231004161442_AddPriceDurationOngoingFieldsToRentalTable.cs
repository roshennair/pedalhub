using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PedalHub.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceDurationOngoingFieldsToRentalTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Rentals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsOngoing",
                table: "Rentals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<float>(
                name: "TotalPrice",
                table: "Rentals",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "IsOngoing",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Rentals");
        }
    }
}
