using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GummyMeter.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCriticToReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCritic",
                table: "Reviews",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Reviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCritic",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Reviews");
        }
    }
}
