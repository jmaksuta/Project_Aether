using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_Aether_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddWorldZone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorldZoneId",
                table: "GameObjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "WorldZones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    SceneName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServerIPAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServerPort = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorldZones", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameObjects_WorldZoneId",
                table: "GameObjects",
                column: "WorldZoneId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameObjects_WorldZones_WorldZoneId",
                table: "GameObjects",
                column: "WorldZoneId",
                principalTable: "WorldZones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameObjects_WorldZones_WorldZoneId",
                table: "GameObjects");

            migrationBuilder.DropTable(
                name: "WorldZones");

            migrationBuilder.DropIndex(
                name: "IX_GameObjects_WorldZoneId",
                table: "GameObjects");

            migrationBuilder.DropColumn(
                name: "WorldZoneId",
                table: "GameObjects");
        }
    }
}
