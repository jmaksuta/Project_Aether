using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_Aether_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPositionToGameObject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "xPosition",
                table: "GameObjects",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "yPosition",
                table: "GameObjects",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "zPosition",
                table: "GameObjects",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xPosition",
                table: "GameObjects");

            migrationBuilder.DropColumn(
                name: "yPosition",
                table: "GameObjects");

            migrationBuilder.DropColumn(
                name: "zPosition",
                table: "GameObjects");
        }
    }
}
