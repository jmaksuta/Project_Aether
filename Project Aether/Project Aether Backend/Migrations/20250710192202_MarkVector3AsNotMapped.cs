using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_Aether_Backend.Migrations
{
    /// <inheritdoc />
    public partial class MarkVector3AsNotMapped : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameObjects_PlayerProfiles_PlayerProfileId",
                table: "GameObjects");

            migrationBuilder.DropForeignKey(
                name: "FK_OnlineConnections_AspNetUsers_UserId",
                table: "OnlineConnections");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerProfiles_AspNetUsers_UserId",
                table: "PlayerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreTransactions_AspNetUsers_UserId",
                table: "StoreTransactions");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PlayerProfiles",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerProfiles_UserId",
                table: "PlayerProfiles",
                newName: "IX_PlayerProfiles_ApplicationUserId");

            migrationBuilder.AddColumn<int>(
                name: "PlayerProfileId1",
                table: "GameObjects",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateRegistered",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateRegistered = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerProfile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlayerName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerProfile_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameObjects_PlayerProfileId1",
                table: "GameObjects",
                column: "PlayerProfileId1");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_userId",
                table: "AspNetUsers",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerProfile_UserId",
                table: "PlayerProfile",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_User_userId",
                table: "AspNetUsers",
                column: "userId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameObjects_PlayerProfile_PlayerProfileId",
                table: "GameObjects",
                column: "PlayerProfileId",
                principalTable: "PlayerProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GameObjects_PlayerProfiles_PlayerProfileId1",
                table: "GameObjects",
                column: "PlayerProfileId1",
                principalTable: "PlayerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OnlineConnections_User_UserId",
                table: "OnlineConnections",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerProfiles_AspNetUsers_ApplicationUserId",
                table: "PlayerProfiles",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreTransactions_User_UserId",
                table: "StoreTransactions",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_User_userId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_GameObjects_PlayerProfile_PlayerProfileId",
                table: "GameObjects");

            migrationBuilder.DropForeignKey(
                name: "FK_GameObjects_PlayerProfiles_PlayerProfileId1",
                table: "GameObjects");

            migrationBuilder.DropForeignKey(
                name: "FK_OnlineConnections_User_UserId",
                table: "OnlineConnections");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerProfiles_AspNetUsers_ApplicationUserId",
                table: "PlayerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreTransactions_User_UserId",
                table: "StoreTransactions");

            migrationBuilder.DropTable(
                name: "PlayerProfile");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropIndex(
                name: "IX_GameObjects_PlayerProfileId1",
                table: "GameObjects");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_userId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PlayerProfileId1",
                table: "GameObjects");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "PlayerProfiles",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerProfiles_ApplicationUserId",
                table: "PlayerProfiles",
                newName: "IX_PlayerProfiles_UserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateRegistered",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddForeignKey(
                name: "FK_GameObjects_PlayerProfiles_PlayerProfileId",
                table: "GameObjects",
                column: "PlayerProfileId",
                principalTable: "PlayerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OnlineConnections_AspNetUsers_UserId",
                table: "OnlineConnections",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerProfiles_AspNetUsers_UserId",
                table: "PlayerProfiles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreTransactions_AspNetUsers_UserId",
                table: "StoreTransactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
