using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Project_Aether_Backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateRegistered = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.Id);
                });

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
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateRegistered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

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
                    ServerPort = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorldZones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OnlineConnections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConnectionId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ConnectedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastActivity = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlineConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnlineConnections_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlayerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "GameObjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ObjectType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    WorldZoneId = table.Column<int>(type: "int", nullable: false),
                    PrefabName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrefabConfigData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    xPosition = table.Column<double>(type: "float", nullable: false),
                    yPosition = table.Column<double>(type: "float", nullable: false),
                    zPosition = table.Column<double>(type: "float", nullable: false),
                    GameObjectType = table.Column<int>(type: "int", nullable: false),
                    archetypeDefinitionId = table.Column<int>(type: "int", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: true, defaultValue: 1),
                    Experience = table.Column<long>(type: "bigint", nullable: true, defaultValue: 0L),
                    Health = table.Column<int>(type: "int", nullable: true, defaultValue: 100),
                    Mana = table.Column<int>(type: "int", nullable: true, defaultValue: 50),
                    GameCharacter_InventoryId = table.Column<int>(type: "int", nullable: true),
                    profilePictureId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PlayerProfileId = table.Column<int>(type: "int", nullable: true),
                    InventoryId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true, defaultValue: 1),
                    ItemType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsStackable = table.Column<bool>(type: "bit", nullable: true),
                    InventoryItem_InventoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameObjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameObjects_Archetypes_archetypeDefinitionId",
                        column: x => x.archetypeDefinitionId,
                        principalTable: "Archetypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_GameObjects_Inventories_GameCharacter_InventoryId",
                        column: x => x.GameCharacter_InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameObjects_Inventories_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameObjects_Inventories_InventoryItem_InventoryId",
                        column: x => x.InventoryItem_InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameObjects_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameObjects_WorldZones_WorldZoneId",
                        column: x => x.WorldZoneId,
                        principalTable: "WorldZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.CreateIndex(
                name: "IX_Archetypes_StoreItemId",
                table: "Archetypes",
                column: "StoreItemId",
                unique: true,
                filter: "[StoreItemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GameObjects_archetypeDefinitionId",
                table: "GameObjects",
                column: "archetypeDefinitionId",
                unique: true,
                filter: "[archetypeDefinitionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GameObjects_GameCharacter_InventoryId",
                table: "GameObjects",
                column: "GameCharacter_InventoryId",
                unique: true,
                filter: "[InventoryId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GameObjects_InventoryId",
                table: "GameObjects",
                column: "InventoryId",
                unique: true,
                filter: "[InventoryId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GameObjects_InventoryItem_InventoryId",
                table: "GameObjects",
                column: "InventoryItem_InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_GameObjects_PlayerProfileId",
                table: "GameObjects",
                column: "PlayerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_GameObjects_WorldZoneId",
                table: "GameObjects",
                column: "WorldZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_OnlineConnections_UserId",
                table: "OnlineConnections",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerProfiles_UserId",
                table: "PlayerProfiles",
                column: "UserId",
                unique: true);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "GameObjects");

            migrationBuilder.DropTable(
                name: "OnlineConnections");

            migrationBuilder.DropTable(
                name: "StoreTransactionItems");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Archetypes");

            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.DropTable(
                name: "PlayerProfiles");

            migrationBuilder.DropTable(
                name: "WorldZones");

            migrationBuilder.DropTable(
                name: "StoreTransactions");

            migrationBuilder.DropTable(
                name: "StoreItems");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
