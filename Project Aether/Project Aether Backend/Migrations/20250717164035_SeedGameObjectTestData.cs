using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_Aether_Backend.Migrations
{
    /// <inheritdoc />
    public partial class SeedGameObjectTestData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GameObjects",
                columns: new[] { "Id", "Description", "GameObjectType", "IsActive", "Name", "ObjectType", "PrefabConfigData", "PrefabName", "WorldZoneId", "xPosition", "yPosition", "zPosition" },
                values: new object[] { 1, "This is a test object for interaction.", 0, true, "Test Object", "NonPlayerCharacter", "{\"Color\":\"Red\",\"Diameter\":0.25,\"InteractionMessage\":\"You have interacted with the test sphere.\"}", "InteractableSphere", 1, 1.0, 1.0, 1.0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GameObjects",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
