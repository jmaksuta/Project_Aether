using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Project_Aether_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddArchetypesAndStoreClasses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CharacterClass",
                table: "GameObjects");

            migrationBuilder.AddColumn<int>(
                name: "archetypeDefinitionId",
                table: "GameObjects",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StoreItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    quantityAvailable = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    isTaxed = table.Column<bool>(type: "bit", nullable: false),
                    taxRate = table.Column<double>(type: "float", nullable: false),
                    taxAmount = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoreTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreTransactions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Archetypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvatarImageId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StoreItemId = table.Column<int>(type: "int", nullable: true),
                    BaseHealth = table.Column<int>(type: "int", nullable: false),
                    BaseMana = table.Column<int>(type: "int", nullable: false),
                    BaseStrength = table.Column<int>(type: "int", nullable: false),
                    BaseAgility = table.Column<int>(type: "int", nullable: false),
                    BaseIntelligence = table.Column<int>(type: "int", nullable: false),
                    BaseIntuition = table.Column<int>(type: "int", nullable: false),
                    BaseCharisma = table.Column<int>(type: "int", nullable: false),
                    BaseLuck = table.Column<int>(type: "int", nullable: false),
                    BaseDefense = table.Column<int>(type: "int", nullable: false),
                    BaseDodge = table.Column<int>(type: "int", nullable: false),
                    BaseSpeed = table.Column<int>(type: "int", nullable: false),
                    StartingAbilities = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartingEquipment = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Archetypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Archetypes_StoreItems_StoreItemId",
                        column: x => x.StoreItemId,
                        principalTable: "StoreItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StoreTransactionItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreTransactionId = table.Column<int>(type: "int", nullable: false),
                    StoreItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<double>(type: "float", nullable: false),
                    TotalPrice = table.Column<double>(type: "float", nullable: false),
                    TaxAmount = table.Column<double>(type: "float", nullable: false),
                    TaxRate = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreTransactionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreTransactionItems_StoreItems_StoreItemId",
                        column: x => x.StoreItemId,
                        principalTable: "StoreItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreTransactionItems_StoreTransactions_StoreTransactionId",
                        column: x => x.StoreTransactionId,
                        principalTable: "StoreTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Archetypes",
                columns: new[] { "Id", "AvatarImageId", "BaseAgility", "BaseCharisma", "BaseDefense", "BaseDodge", "BaseHealth", "BaseIntelligence", "BaseIntuition", "BaseLuck", "BaseMana", "BaseSpeed", "BaseStrength", "Description", "Name", "StartingAbilities", "StartingEquipment", "StoreItemId" },
                values: new object[,]
                {
                    { 1, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "Grew up in the forgotten, grimy industrial districts where old steampunk machinery and early Magic-Theory tech from the 80s/90s era still clunk along. You learned to fix things with a wrench and a basic understanding of Aetheric circuits.", "Streetwise Arcane Mechanic", "[\"Basic Alchemy\",\"Arcane Mechanics\"]", "[\"a \\u0027Boombox Golem\\u0027 blueprint\",\"a \\u0027CRT Scrying Monitor\\u0027 schematics\"]", null },
                    { 2, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "Hailing from a powerful, Magic-Theory corporate family that builds the city's sleek, modern infrastructure. You were trained in cutting-edge Magic-Theory but were recently exiled or discredited due to a scandalous project failure (perhaps an early encounter with the Static Cascade).", "Corporate Scion / Disgraced Magitech Engineer", "[]", "[]", null },
                    { 3, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "Came to the metropolis from the untamed 'Wilds' – pockets of ancient, primeval fantasy landscapes that exist just beyond or hidden within the city's boundaries. You have a deeper connection to raw Aether and elemental spirits.", "Aether-Touched Outlander", "[]", "[]", null },
                    { 4, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "Grew up immersed in the underground subcultures that celebrate the 80s/90s aesthetic – from synth-wave musical guilds to grunge alchemist collectives. You're known for your unique alchemical mixes and your connection to the street art scene.", "Arcade Alchemist / Subculture Guru", "[]", "[]", null },
                    { 5, "", 0, 0, 0, 0, 100, 0, 0, 0, 30, 0, 0, "You were once part of a city-sanctioned Magical Response Unit (MRU) or a private security firm, trained in close-quarters Aether-combat and the tactical application of Magic-Theory weaponry. You're disciplined and practical, but a past incident involving Static Cascade interference (or perhaps the MRU's rigid methods) led to your discharge or disillusionment", "Arcane Enforcer", "[\"Basic Melee Attack\",\"Shield Bash\"]", "[]", null },
                    { 6, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "You thrive in the underbelly of the city, using your unique blend of Aetheric manipulation and understanding of Magic-Theory vulnerabilities to bypass magical security systems, hack into enchanted networks(the \"Nether-Net\"), or acquire rare alchemical components.You're nimble, stealthy, and prefer to work in the shadows.", "Net-Dancer / Aetheric Infiltrator", "[]", "[]", null },
                    { 7, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "You possess a rare form of Aetheric divination, allowing you to read the subtle currents and \"fate echoes\" of the city through methods like Arcane Tarot, Scrying Pool reflections, or Aether-charged crystal balls.You see glimpses of possible futures and past events, making you an invaluable, albeit cryptic, source of information.The Static Cascade is making your readings chaotic and dangerous.", "Urban Soothsayer / Resonance Reader", "[]", "[]", null },
                    { 8, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "You are attuned to the more spiritual side of the city's Magic-Theory, capable of communing with benevolent urban spirits, ancient elemental guardians, or the very \"soul\" of the metropolis itself. You use your connection to mend, heal, and restore, but the Static Cascade is disrupting these vital connections.", "Spirit Channeler / Aetheric Empath", "[]", "[]", null },
                    { 9, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "You've honed your body and mind to channel raw Aetheric energy through precise movements and martial arts techniques, turning your fists and feet into conduits of magical force. You might belong to an underground dojo hidden in a renovated 80s gym, where ancient techniques are taught alongside modern interpretations of Magic-Theory. The Static Cascade tests your control, making your powers unpredictable.", "Urban Mystic / Aether-Prowler", "[]", "[]", null },
                    { 10, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "You honed your skills in navigating the city's hidden passages and bypassing its arcane security systems, often for personal gain or to \"liberate\" forgotten relics. You're nimble, resourceful, and have a knack for getting into places others can't. Your Resonance allows you to sense weak points in magical wards.", "Shadow Weasel", "[]", "[]", null },
                    { 11, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "You're a natural showperson, using your innate Resonance to amplify your performances and evoke powerful emotions, or even minor magical effects, through music, voice, or art. You might be a budding synth-rock star, a captivating street magician, or a charismatic underground radio DJ. The Static Cascade causes strange feedback loops in magical sound/light.", "Aetheric Amplifier", "[]", "[]", null },
                    { 12, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "You're a seasoned \"Mana-Runner\" (smuggler/courier) who operates on the fringes of the city's Magic-Theory regulations.You pilot a \"mana-fueled\" vehicle (think a classic 80s muscle car or a clunky, modified hover-van, powered by volatile alchemical mixtures) through dangerous, unpatrolled Aetheric zones, delivering illicit alchemical goods, sensitive data stored in memory crystals, or even \"off-grid\" passengers.You prioritize personal freedom and profit, but possess a surprising moral compass when truly tested.The Static Cascade is making your routes unpredictable and perilous.", "Mana-Runner", "[]", "[]", null },
                    { 13, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "You're a charismatic socialite, entrepreneur, or \"fixer\" who understands the power of charm, influence, and shrewd deals in the city's intertwined magical and corporate circles.You manage (or used to manage) a high-end establishment – perhaps a magically-gambling den, an exclusive neon-lit club, or a black-market Aetheric trading post.You have a silver tongue and a knack for turning any situation to your advantage, often juggling multiple, sometimes conflicting, alliances.", "Aether-Baron", "[]", "[]", null },
                    { 14, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "You were raised in relative obscurity, perhaps in a quiet, isolated corner of the city or even just outside it, seemingly disconnected from the grand Magic-Theory networks.However, you've always felt a strange pull, a deeper connection to the city's Aetheric currents than others.You possess an untapped potential for Arcane Resonance, destined for something greater than your humble beginnings.You are idealistic and driven by a desire to help, but naive about the city's true complexities.", "Resonance Prodigy", "[]", "[]", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameObjects_archetypeDefinitionId",
                table: "GameObjects",
                column: "archetypeDefinitionId",
                unique: true,
                filter: "[archetypeDefinitionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Archetypes_StoreItemId",
                table: "Archetypes",
                column: "StoreItemId",
                unique: true,
                filter: "[StoreItemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTransactionItems_StoreItemId",
                table: "StoreTransactionItems",
                column: "StoreItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTransactionItems_StoreTransactionId",
                table: "StoreTransactionItems",
                column: "StoreTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTransactions_UserId",
                table: "StoreTransactions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameObjects_Archetypes_archetypeDefinitionId",
                table: "GameObjects",
                column: "archetypeDefinitionId",
                principalTable: "Archetypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameObjects_Archetypes_archetypeDefinitionId",
                table: "GameObjects");

            migrationBuilder.DropTable(
                name: "Archetypes");

            migrationBuilder.DropTable(
                name: "StoreTransactionItems");

            migrationBuilder.DropTable(
                name: "StoreItems");

            migrationBuilder.DropTable(
                name: "StoreTransactions");

            migrationBuilder.DropIndex(
                name: "IX_GameObjects_archetypeDefinitionId",
                table: "GameObjects");

            migrationBuilder.DropColumn(
                name: "archetypeDefinitionId",
                table: "GameObjects");

            migrationBuilder.AddColumn<string>(
                name: "CharacterClass",
                table: "GameObjects",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
