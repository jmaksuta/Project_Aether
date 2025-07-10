using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Project_Aether_Backend.Migrations
{
    /// <inheritdoc />
    public partial class PopulateWorldZoneData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ServerPort",
                table: "WorldZones",
                type: "int",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.InsertData(
                table: "WorldZones",
                columns: new[] { "Id", "Description", "Name", "SceneName", "ServerIPAddress", "ServerPort", "ZoneId" },
                values: new object[,]
                {
                    { 1, "The character's starting point", "Node Forge", "04_NodeForge_Intro", "192.168.1.147", 55002, 1 },
                    { 2, "The Forest Zone", "Forest Zone", "05_ForestZone", "192.168.1.147", 55003, 2 },
                    { 3, "The Dungeon Zone", "Dungeon Zone", "05_DungeonZone", "192.168.1.147", 55004, 3 },
                    { 4, "The Main City Zone", "Main City", "05_MainCityScene", "192.168.1.147", 55005, 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "WorldZones",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "WorldZones",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "WorldZones",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "WorldZones",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.AlterColumn<short>(
                name: "ServerPort",
                table: "WorldZones",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
