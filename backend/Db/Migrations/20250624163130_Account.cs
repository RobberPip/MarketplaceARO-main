using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Db.Migrations {
    /// <inheritdoc />
    public partial class Account : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_Accounts_AccountId",
                schema: "public",
                table: "Profiles");


            migrationBuilder.DropColumn(
                name: "Description",
                schema: "public",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "LegalAddress",
                schema: "public",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "LegalName",
                schema: "public",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "MainActivity",
                schema: "public",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "PublicName",
                schema: "public",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Website",
                schema: "public",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                schema: "public",
                table: "Profiles",
                newName: "ActiveAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Profiles_AccountId",
                schema: "public",
                table: "Profiles",
                newName: "IX_Profiles_ActiveAccountId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "public",
                table: "Accounts",
                type: "text",
                nullable: false,
                defaultValue: "Pending",
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Blank");

            migrationBuilder.AddColumn<Guid>(
                name: "BrandId",
                schema: "public",
                table: "Accounts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeveloper",
                schema: "public",
                table: "Accounts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OGRN",
                schema: "public",
                table: "Accounts",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                schema: "public",
                table: "Accounts",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Brands",
                schema: "public",
                columns: table => new {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AvatarId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "Pending"),
                    PublicName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Website = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Brands_Files_AvatarId",
                        column: x => x.AvatarId,
                        principalSchema: "public",
                        principalTable: "Files",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Invitations",
                schema: "public",
                columns: table => new {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Role = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Invitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invitations_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "public",
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invitations_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "public",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "MainActivity",
                schema: "public",
                columns: table => new {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_MainActivity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublicList",
                schema: "public",
                columns: table => new {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_PublicList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BrandMainActivity",
                schema: "public",
                columns: table => new {
                    BrandsId = table.Column<Guid>(type: "uuid", nullable: false),
                    MainActivitiesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_BrandMainActivity", x => new { x.BrandsId, x.MainActivitiesId });
                    table.ForeignKey(
                        name: "FK_BrandMainActivity_Brands_BrandsId",
                        column: x => x.BrandsId,
                        principalSchema: "public",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandMainActivity_MainActivity_MainActivitiesId",
                        column: x => x.MainActivitiesId,
                        principalSchema: "public",
                        principalTable: "MainActivity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BrandPublicList",
                schema: "public",
                columns: table => new {
                    BrandsId = table.Column<Guid>(type: "uuid", nullable: false),
                    PublicListsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_BrandPublicList", x => new { x.BrandsId, x.PublicListsId });
                    table.ForeignKey(
                        name: "FK_BrandPublicList_Brands_BrandsId",
                        column: x => x.BrandsId,
                        principalSchema: "public",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandPublicList_PublicList_PublicListsId",
                        column: x => x.PublicListsId,
                        principalSchema: "public",
                        principalTable: "PublicList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "MainActivity",
                columns: new[] { "Id", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0209e092-b762-4eb2-b612-be5d7621b0d8"), "Судостроение", null },
                    { new Guid("1cce5e9d-5a10-4070-87a1-1e55da6123e9"), "Автомобилестроение", null },
                    { new Guid("36d2864f-f2d2-482b-af12-eac63cfdeb3c"), "Добыча полезных ископаемых", null },
                    { new Guid("6260291c-2a00-4288-8723-a1cb4f5109a5"), "Разработка программного обеспечения", null },
                    { new Guid("6642069a-82fe-4d66-ab46-ab8ec8d283f3"), "Другое", null },
                    { new Guid("71988b54-8916-4768-b134-6aacf1e2a49b"), "Строительство", null },
                    { new Guid("7d038279-798f-404d-8886-f027e39a6c7e"), "Эксплуатация", null },
                    { new Guid("bd3af7bd-e06a-4973-a946-70ce9fba8453"), "Машиностроение", null },
                    { new Guid("e735efad-5bbd-4669-9b0d-1b2d9e11b338"), "Проектирование", null }
                });


            migrationBuilder.InsertData(
                schema: "public",
                table: "PublicList",
                columns: new[] { "Id", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("735e08b8-bb15-4809-ab49-17610648fc98"), "НОТЕХ", null },
                    { new Guid("9424be98-4ce3-449c-9549-74f225a90373"), "НОПРИЗ", null },
                    { new Guid("aa18ef2b-e9f4-4329-ba58-c3c975a73515"), "НОТИМ", null },
                    { new Guid("dd0b44b7-ccba-4621-a829-fd2ab62127fd"), "НОСТРОЙ", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_BrandId",
                schema: "public",
                table: "Accounts",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandMainActivity_MainActivitiesId",
                schema: "public",
                table: "BrandMainActivity",
                column: "MainActivitiesId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandPublicList_PublicListsId",
                schema: "public",
                table: "BrandPublicList",
                column: "PublicListsId");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_AvatarId",
                schema: "public",
                table: "Brands",
                column: "AvatarId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_AccountId",
                schema: "public",
                table: "Invitations",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_ProfileId",
                schema: "public",
                table: "Invitations",
                column: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Brands_BrandId",
                schema: "public",
                table: "Accounts",
                column: "BrandId",
                principalSchema: "public",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_Accounts_ActiveAccountId",
                schema: "public",
                table: "Profiles",
                column: "ActiveAccountId",
                principalSchema: "public",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Brands_BrandId",
                schema: "public",
                table: "Accounts");


            migrationBuilder.DropTable(
                name: "BrandMainActivity",
                schema: "public");

            migrationBuilder.DropTable(
                name: "BrandPublicList",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Invitations",
                schema: "public");

            migrationBuilder.DropTable(
                name: "MainActivity",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Brands",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PublicList",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_BrandId",
                schema: "public",
                table: "Accounts");


            migrationBuilder.DropColumn(
                name: "BrandId",
                schema: "public",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "IsDeveloper",
                schema: "public",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "OGRN",
                schema: "public",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "ShortName",
                schema: "public",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "ActiveAccountId",
                schema: "public",
                table: "Profiles",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Profiles_ActiveAccountId",
                schema: "public",
                table: "Profiles",
                newName: "IX_Profiles_AccountId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "public",
                table: "Accounts",
                type: "text",
                nullable: false,
                defaultValue: "Blank",
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Pending");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "public",
                table: "Accounts",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalAddress",
                schema: "public",
                table: "Accounts",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalName",
                schema: "public",
                table: "Accounts",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainActivity",
                schema: "public",
                table: "Accounts",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicName",
                schema: "public",
                table: "Accounts",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                schema: "public",
                table: "Accounts",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_Accounts_AccountId",
                schema: "public",
                table: "Profiles",
                column: "AccountId",
                principalSchema: "public",
                principalTable: "Accounts",
                principalColumn: "Id");
        }
    }
}
