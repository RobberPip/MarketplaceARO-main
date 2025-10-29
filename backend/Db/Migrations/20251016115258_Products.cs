using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Db.Migrations {
    /// <inheritdoc />
    public partial class Products : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {

            migrationBuilder.DropColumn(
                name: "LegacySoftwareId",
                schema: "public",
                table: "Reviews");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                schema: "public",
                table: "Reviews",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "AttributeMeta",
                schema: "public",
                columns: table => new {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DataType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    IsFilterable = table.Column<bool>(type: "boolean", nullable: false),
                    IsSortable = table.Column<bool>(type: "boolean", nullable: false),
                    AvailableValues = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_AttributeMeta", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductsData",
                schema: "public",
                columns: table => new {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_ProductsData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductTypes",
                schema: "public",
                columns: table => new {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ShortName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CombinedName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_ProductTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "public",
                columns: table => new {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "Draft"),
                    Website = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductDataId = table.Column<Guid>(type: "uuid", nullable: true),
                    LikesCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ReviewsCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    AverageRating = table.Column<decimal>(type: "numeric(3,2)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Accounts_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "public",
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_ProductTypes_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "ProductTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_ProductsData_ProductDataId",
                        column: x => x.ProductDataId,
                        principalSchema: "public",
                        principalTable: "ProductsData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductFiles",
                schema: "public",
                columns: table => new {
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_ProductFiles", x => new { x.ProductId, x.FileId });
                    table.ForeignKey(
                        name: "FK_ProductFiles_Files_FileId",
                        column: x => x.FileId,
                        principalSchema: "public",
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductFiles_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "public",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rightholders",
                schema: "public",
                columns: table => new {
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Rightholders", x => new { x.ProductId, x.AccountId });
                    table.ForeignKey(
                        name: "FK_Rightholders_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "public",
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rightholders_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "public",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });



            migrationBuilder.InsertData(
                schema: "public",
                table: "AttributeMeta",
                columns: new[] { "Id", "AvailableValues", "DataType", "IsFilterable", "IsSortable", "Name", "Unit" },
                values: new object[] { new Guid("d346573c-be0e-4f8c-91fb-053ce6166564"), "[]", "string", false, false, "VideoUrl", null });



            migrationBuilder.InsertData(
                schema: "public",
                table: "ProductTypes",
                columns: new[] { "Id", "CombinedName", "CreatedAt", "FullName", "ShortName", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("240f5397-0be0-4854-85ef-42f26be33024"), "Системы управление жизненным циклом продукции (PLM)", new DateTime(2025, 10, 16, 11, 52, 56, 644, DateTimeKind.Utc).AddTicks(6499), "Системы управление жизненным циклом продукции", "PLM", null },
                    { new Guid("294e28df-5b09-4757-80e5-7bb5fd05eed2"), "Системы управления проектами и календарно-сетевого планирования (PMS)", new DateTime(2025, 10, 16, 11, 52, 56, 644, DateTimeKind.Utc).AddTicks(6504), "Системы управления проектами и календарно-сетевого планирования", "PMS", null },
                    { new Guid("2c5d1ae9-a5c5-4c57-93e0-00953d5bc440"), "Системы виртуальной и дополненной реальности (AR/VR)", new DateTime(2025, 10, 16, 11, 52, 56, 644, DateTimeKind.Utc).AddTicks(6057), "Системы виртуальной и дополненной реальности", "AR/VR", null },
                    { new Guid("2d64919b-7f45-40cd-8583-1dcd51c1664f"), "Средства текстового и графического создания и редактирования (CGT)", new DateTime(2025, 10, 16, 11, 52, 56, 644, DateTimeKind.Utc).AddTicks(6541), "Средства текстового и графического создания и редактирования", "CGT", null },
                    { new Guid("49b03052-3b57-4ce4-8109-73b0e84da0b4"), "Системы электронного документооборота (СЭД)", new DateTime(2025, 10, 16, 11, 52, 56, 644, DateTimeKind.Utc).AddTicks(6533), "Системы электронного документооборота", "СЭД", null },
                    { new Guid("4ee7d85e-96f7-4621-96d0-766c59d4c7b9"), "Системы инженерного анализа и расчетов (CAE)", new DateTime(2025, 10, 16, 11, 52, 56, 644, DateTimeKind.Utc).AddTicks(6063), "Системы инженерного анализа и расчетов", "CAE", null },
                    { new Guid("574037a8-797b-4b21-8f4b-91329bd397f2"), "Среда общих данных (CDE)", new DateTime(2025, 10, 16, 11, 52, 56, 644, DateTimeKind.Utc).AddTicks(6539), "Среда общих данных", "CDE", null },
                    { new Guid("67384899-9f89-49d0-ba2d-52a03c2c8685"), "Системы CAD проектирования (CAD)", new DateTime(2025, 10, 16, 11, 52, 56, 644, DateTimeKind.Utc).AddTicks(6054), "Системы CAD проектирования", "CAD", null },
                    { new Guid("67f06e81-ba63-4314-89d5-c8406ead2161"), "Системы технического обслуживания и ремонта (ТОиР)", new DateTime(2025, 10, 16, 11, 52, 56, 644, DateTimeKind.Utc).AddTicks(6470), "Системы технического обслуживания и ремонта", "ТОиР", null },
                    { new Guid("6abadbcc-215d-4d69-abba-220641a3962a"), "Системы управления строительством и строительного контроля (CMS)", new DateTime(2025, 10, 16, 11, 52, 56, 644, DateTimeKind.Utc).AddTicks(6516), "Системы управления строительством и строительного контроля", "CMS", null },
                    { new Guid("80dc9124-7a1c-4505-a620-12e5e09bab49"), "Средства технологической подготовки производства (CAPP)", new DateTime(2025, 10, 16, 11, 52, 56, 644, DateTimeKind.Utc).AddTicks(6544), "Средства технологической подготовки производства", "CAPP", null },
                    { new Guid("8dbc773b-49e2-44d1-b93e-cb0b2fed998a"), "Серверное и связующее программное обеспечение (MWS)", new DateTime(2025, 10, 16, 11, 52, 56, 644, DateTimeKind.Utc).AddTicks(6050), "Серверное и связующее программное обеспечение", "MWS", null },
                    { new Guid("967b94ca-0b26-4911-92d8-91b9b27e31d1"), "Системы управления ресурсами предприятия (ERP)", new DateTime(2025, 10, 16, 11, 52, 56, 644, DateTimeKind.Utc).AddTicks(6514), "Системы управления ресурсами предприятия", "ERP", null },
                    { new Guid("a41c418f-414f-419e-bfc7-2445276b4bd9"), "Системы для оценки стоимости строительства (CCE)", new DateTime(2025, 10, 16, 11, 52, 56, 644, DateTimeKind.Utc).AddTicks(6061), "Системы для оценки стоимости строительства", "CCE", null },
                    { new Guid("aa3da3b3-7dc8-4e75-a193-ab1c38e5ca5b"), "Системы диспетчерского управления и сбора данных (SCADA)", new DateTime(2025, 10, 16, 11, 52, 56, 644, DateTimeKind.Utc).AddTicks(6059), "Системы диспетчерского управления и сбора данных", "SCADA", null },
                    { new Guid("ab943fda-32e4-4c60-8326-0d190d1606bb"), "Геоинформационные и навигационные системы (GIS)", new DateTime(2025, 10, 16, 11, 52, 56, 644, DateTimeKind.Utc).AddTicks(5788), "Геоинформационные и навигационные системы", "GIS", null },
                    { new Guid("c15cf32e-d243-4ddd-9157-08839611c133"), "Системы управления процессами охраны труда, промышленной и пожарной безопасности (ОТиПБ)", new DateTime(2025, 10, 16, 11, 52, 56, 644, DateTimeKind.Utc).AddTicks(6511), "Системы управления процессами охраны труда, промышленной и пожарной безопасности", "ОТиПБ", null },
                    { new Guid("cecf27ad-0585-424c-9641-ca50677d9106"), "Системы управления требованиями (RMS)", new DateTime(2025, 10, 16, 11, 52, 56, 644, DateTimeKind.Utc).AddTicks(6521), "Системы управления требованиями", "RMS", null },
                    { new Guid("cf91ecf8-057f-40d0-bcf2-c6223087ead5"), "Средства управления производственными процессами (MES)", new DateTime(2025, 10, 16, 11, 52, 56, 644, DateTimeKind.Utc).AddTicks(6547), "Средства управления производственными процессами", "MES", null },
                    { new Guid("d714a11c-0a64-468f-b848-c2ed7a4e57ea"), "Системы компьютерного зрения (CVS)", new DateTime(2025, 10, 16, 11, 52, 56, 644, DateTimeKind.Utc).AddTicks(6065), "Системы компьютерного зрения", "CVS", null },
                    { new Guid("f05831e5-2d16-426d-8b7d-be6b480bc867"), "Системы управления инженерными данными об изделии (PDM)", new DateTime(2025, 10, 16, 11, 52, 56, 644, DateTimeKind.Utc).AddTicks(6502), "Системы управления инженерными данными об изделии", "PDM", null },
                    { new Guid("fb96ccd0-25f5-43a3-b755-e3914468b66d"), "Справочные системы, справочники, библиотеки и базы данных (RDM)", new DateTime(2025, 10, 16, 11, 52, 56, 644, DateTimeKind.Utc).AddTicks(6537), "Справочные системы, справочники, библиотеки и базы данных", "RDM", null }
                });



            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProductId",
                schema: "public",
                table: "Reviews",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFiles_FileId",
                schema: "public",
                table: "ProductFiles",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_OwnerId",
                schema: "public",
                table: "Products",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductDataId",
                schema: "public",
                table: "Products",
                column: "ProductDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TypeId",
                schema: "public",
                table: "Products",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Rightholders_AccountId",
                schema: "public",
                table: "Rightholders",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Products_ProductId",
                schema: "public",
                table: "Reviews",
                column: "ProductId",
                principalSchema: "public",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Products_ProductId",
                schema: "public",
                table: "Reviews");

            migrationBuilder.DropTable(
                name: "AttributeMeta",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ProductFiles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Rightholders",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ProductTypes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ProductsData",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ProductId",
                schema: "public",
                table: "Reviews");



            migrationBuilder.DropColumn(
                name: "ProductId",
                schema: "public",
                table: "Reviews");

            migrationBuilder.AddColumn<int>(
                name: "LegacySoftwareId",
                schema: "public",
                table: "Reviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
