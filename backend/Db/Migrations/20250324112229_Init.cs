using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Db.Migrations {
    /// <inheritdoc />
    public partial class Init : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            var assembly = Assembly.GetExecutingAssembly();

            void ExecuteMigration(string resourceName) {
                using var stream = assembly.GetManifestResourceStream(resourceName)
                               ?? throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");
                using var reader = new StreamReader(stream);

                var sql = reader.ReadToEnd();
                migrationBuilder.Sql(sql);
            }

            var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
            if (isDevelopment) {
                ExecuteMigration("Db.Migrations.Scripts.LegacyRegistrySeedData.sql");
            }
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "Accounts",
                schema: "public",
                columns: table => new {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "Blank"),
                    Type = table.Column<string>(type: "text", nullable: false, defaultValue: "Regular"),
                    INN = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    FullName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PublicName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    LegalName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Website = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    MainActivity = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    LegalAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    KPP = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                schema: "public",
                columns: table => new {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                schema: "public",
                columns: table => new {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                schema: "public",
                columns: table => new {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "Init"),
                    IsRegistry = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyIndustries",
                schema: "public",
                columns: table => new {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_CompanyIndustries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                schema: "public",
                columns: table => new {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    Content = table.Column<byte[]>(type: "bytea", nullable: true),
                    Path = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfileRoles",
                schema: "public",
                columns: table => new {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_ProfileRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                schema: "public",
                columns: table => new {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "public",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                schema: "public",
                columns: table => new {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                schema: "public",
                columns: table => new {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                schema: "public",
                columns: table => new {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "public",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "public",
                columns: table => new {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                schema: "public",
                columns: table => new {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Login = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AvatarId = table.Column<Guid>(type: "uuid", nullable: true),
                    CurrentCompanyId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProfileIndustryId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProfileRoleId = table.Column<Guid>(type: "uuid", nullable: true),
                    PersonalDataPolitics = table.Column<bool>(type: "boolean", nullable: false),
                    PersonalDataAgreement = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profiles_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "public",
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Profiles_Companies_CurrentCompanyId",
                        column: x => x.CurrentCompanyId,
                        principalSchema: "public",
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Profiles_CompanyIndustries_ProfileIndustryId",
                        column: x => x.ProfileIndustryId,
                        principalSchema: "public",
                        principalTable: "CompanyIndustries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Profiles_Files_AvatarId",
                        column: x => x.AvatarId,
                        principalSchema: "public",
                        principalTable: "Files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Profiles_ProfileRoles_ProfileRoleId",
                        column: x => x.ProfileRoleId,
                        principalSchema: "public",
                        principalTable: "ProfileRoles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                schema: "public",
                columns: table => new {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    LegacySoftwareId = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: true),
                    Rate = table.Column<int>(type: "integer", nullable: true),
                    Like = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table => {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Profiles_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "public",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCompanies",
                schema: "public",
                columns: table => new {
                    ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table => {
                    table.PrimaryKey("PK_UserCompanies", x => new { x.CompanyId, x.ProfileId });
                    table.ForeignKey(
                        name: "FK_UserCompanies_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "public",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCompanies_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "public",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("3a265dde-7ada-4ffc-a15a-4d152ea5893b"), "8d7aaa0f-38d9-446d-9a27-b2ded50b51ee", "Admin", "ADMIN" },
                    { new Guid("72b7084b-a821-4447-8d31-b359fa0e9a76"), "76a71e2f-7c45-4c77-b181-12561dce095a", "Developer", "DEVELOPER" },
                    { new Guid("9fe5fe8c-0c77-4ae3-91d6-ea61d6d857fc"), "004e09cf-d37c-4929-ab1f-6dd631dd057f", "Member", "MEMBER" }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UpdatedAt", "UserName" },
                values: new object[] { new Guid("cd225915-7d46-4a10-9368-bcbbb6721b19"), 0, "4c8622db-6cd7-428b-b980-f0bc95637ad4", new DateTime(2024, 11, 30, 21, 0, 0, 0, DateTimeKind.Utc), "seed_user@example.ru", true, false, null, "SEED_USER@EXAMPLE.RU", "SEED_USER@EXAMPLE.RU", "AQAAAAIAAYagAAAAEOCvfWwoUWPzeKCQVoWIfPTyO8Q7Q5F7cA91VlW510kTX5jr1C/BF57zYoS2O5s1ug==", null, false, "69d150c1-6fe2-4c43-b890-86225589dd29", false, new DateTime(2024, 11, 30, 21, 0, 0, 0, DateTimeKind.Utc), "seed_user@example.ru" });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Companies",
                columns: new[] { "Id", "CreatedBy", "Description", "IsRegistry", "Name", "Status", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("0195c7e3-c5c9-7244-bd6b-3c6287a7fc9c"), new Guid("00000000-0000-0000-0000-000000000000"), null, true, "Компания 2", "Approved", new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("0195c7e3-c5c9-7265-8880-83378a36ea65"), new Guid("00000000-0000-0000-0000-000000000000"), null, true, "Компания 1", "Approved", new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("0195c7e3-c5c9-730b-86f5-e32838a3db98"), new Guid("00000000-0000-0000-0000-000000000000"), null, true, "Компания 3", "Approved", new Guid("00000000-0000-0000-0000-000000000000") }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Companies",
                columns: new[] { "Id", "CreatedBy", "Description", "IsRegistry", "Name", "UpdatedBy" },
                values: new object[] { new Guid("0195c7e3-c5c9-7ec5-9c53-0cb4c038c85e"), new Guid("00000000-0000-0000-0000-000000000000"), null, true, "Компания 4", new Guid("00000000-0000-0000-0000-000000000000") });

            migrationBuilder.InsertData(
                schema: "public",
                table: "CompanyIndustries",
                columns: new[] { "Id", "CreatedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("6c1f90b7-7fbd-439b-9b75-1e56159dfb5f"), new DateTime(2024, 11, 30, 21, 0, 0, 0, DateTimeKind.Utc), "IT Экзекуция", new DateTime(2024, 11, 30, 21, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("87915b32-cee6-4eff-8f8c-a25b7a168847"), new DateTime(2024, 11, 30, 21, 0, 0, 0, DateTimeKind.Utc), "Археологические раскопки", new DateTime(2024, 11, 30, 21, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b8f90f8b-0e3b-44f1-bf1e-506c14e6321f"), new DateTime(2024, 11, 30, 21, 0, 0, 0, DateTimeKind.Utc), "Проектирование космических кораблей", new DateTime(2024, 11, 30, 21, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "ProfileRoles",
                columns: new[] { "Id", "CreatedAt", "Key", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0a2e3030-8067-4ecb-82b4-b1ff19836e4e"), new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc), "Student", "Студент", new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("0e11c740-0887-48a4-bf22-17323e199a75"), new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc), "Owner", "Руководитель / Директор", new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("12e6a1e1-bd77-44a1-b129-b4d205b5d116"), new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc), "Staff", "Полевой персонал", new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("31c4d0a3-5027-4f0c-9b9a-f19b573b5f8d"), new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc), "Engineer", "Проектный инженер", new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5a72ec6a-bd92-4769-b7d5-b8ecbaf8d970"), new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc), "Customer", "Представитель заказчика", new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("73f8c366-ec86-4731-a5ea-df213ab42286"), new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc), "Arch", "Архитектор / Дизайнер", new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("74f0c5b5-f0a1-47a3-9a3e-7d659453c05e"), new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc), "Activities", "Операционная деятельность / Управление объектами / Эксплуатация и обслуживание", new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("7dcaad3f-45c4-44b3-8771-9ec0c426acdd"), new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc), "Estimator", "Сметчик / Инженер-сметчик", new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("9c28c660-bcb5-47ec-8ab3-fd2fa58a6236"), new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc), "PreProject ", "Руководитель предпроектной подготовки", new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a1b28eae-8a13-4fdb-839b-55ef9eb12b91"), new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc), "ProjectManager", "Менеджер проекта / Менеджер по качеству", new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b6a0c527-6043-4884-b991-fb9357796f2d"), new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc), "Manager", "Менеджер по тендерам", new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c1c1baf3-7745-4f58-84d9-dcfef2cddf5e"), new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc), "Other", "Другое", new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("cb0d4b6c-5b91-478f-bf38-c77e5f27c855"), new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc), "Development", "Развитие бизнеса", new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("deac9f50-d41d-4521-9c11-032d5b1d4e91"), new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc), "Director", "Руководитель проекта", new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("e7ca9ff9-2878-4401-b22d-feb40c09739c"), new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc), "Foreman", "Бригадир / Прораб", new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("f9b2630e-338d-42d5-88e0-d23f8ff53bfe"), new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc), "Technologies", "Технологии и инновации", new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("fa6533c0-9f43-4e5c-8c07-f489e4e080fe"), new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc), "BIM", "Менеджер VDC / BIM", new DateTime(2025, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Profiles",
                columns: new[] { "Id", "AccountId", "AvatarId", "CreatedAt", "CurrentCompanyId", "Description", "Email", "FirstName", "LastName", "Login", "PersonalDataAgreement", "PersonalDataPolitics", "ProfileIndustryId", "ProfileRoleId", "UpdatedAt" },
                values: new object[] { new Guid("cd225915-7d46-4a10-9368-bcbbb6721b19"), null, null, new DateTime(2024, 11, 30, 21, 0, 0, 0, DateTimeKind.Utc), null, null, "seed_user@example.ru", null, null, "seed_user", false, false, null, null, new DateTime(2024, 11, 30, 21, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                schema: "public",
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("3a265dde-7ada-4ffc-a15a-4d152ea5893b"), new Guid("cd225915-7d46-4a10-9368-bcbbb6721b19") });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "public",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "public",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "public",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "public",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "public",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "public",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "public",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileRoles_Key",
                schema: "public",
                table: "ProfileRoles",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_AccountId",
                schema: "public",
                table: "Profiles",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_AvatarId",
                schema: "public",
                table: "Profiles",
                column: "AvatarId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_CurrentCompanyId",
                schema: "public",
                table: "Profiles",
                column: "CurrentCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_ProfileIndustryId",
                schema: "public",
                table: "Profiles",
                column: "ProfileIndustryId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_ProfileRoleId",
                schema: "public",
                table: "Profiles",
                column: "ProfileRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_AuthorId",
                schema: "public",
                table: "Reviews",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanies_ProfileId",
                schema: "public",
                table: "UserCompanies",
                column: "ProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Reviews",
                schema: "public");

            migrationBuilder.DropTable(
                name: "UserCompanies",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AspNetRoles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AspNetUsers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Profiles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Accounts",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Companies",
                schema: "public");

            migrationBuilder.DropTable(
                name: "CompanyIndustries",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Files",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ProfileRoles",
                schema: "public");
        }
    }
}
