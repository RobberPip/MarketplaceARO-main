using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Db.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProfileOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "public",
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("0195c7e3-c5c9-7244-bd6b-3c6287a7fc9c"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("0195c7e3-c5c9-7265-8880-83378a36ea65"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("0195c7e3-c5c9-730b-86f5-e32838a3db98"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("0195c7e3-c5c9-7ec5-9c53-0cb4c038c85e"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "CompanyIndustries",
                keyColumn: "Id",
                keyValue: new Guid("6c1f90b7-7fbd-439b-9b75-1e56159dfb5f"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "CompanyIndustries",
                keyColumn: "Id",
                keyValue: new Guid("87915b32-cee6-4eff-8f8c-a25b7a168847"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "CompanyIndustries",
                keyColumn: "Id",
                keyValue: new Guid("b8f90f8b-0e3b-44f1-bf1e-506c14e6321f"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0a2e3030-8067-4ecb-82b4-b1ff19836e4e"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0e11c740-0887-48a4-bf22-17323e199a75"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("12e6a1e1-bd77-44a1-b129-b4d205b5d116"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("31c4d0a3-5027-4f0c-9b9a-f19b573b5f8d"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("5a72ec6a-bd92-4769-b7d5-b8ecbaf8d970"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("73f8c366-ec86-4731-a5ea-df213ab42286"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("74f0c5b5-f0a1-47a3-9a3e-7d659453c05e"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("7dcaad3f-45c4-44b3-8771-9ec0c426acdd"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("9c28c660-bcb5-47ec-8ab3-fd2fa58a6236"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1b28eae-8a13-4fdb-839b-55ef9eb12b91"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("b6a0c527-6043-4884-b991-fb9357796f2d"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("c1c1baf3-7745-4f58-84d9-dcfef2cddf5e"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("cb0d4b6c-5b91-478f-bf38-c77e5f27c855"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("deac9f50-d41d-4521-9c11-032d5b1d4e91"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("e7ca9ff9-2878-4401-b22d-feb40c09739c"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("f9b2630e-338d-42d5-88e0-d23f8ff53bfe"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("fa6533c0-9f43-4e5c-8c07-f489e4e080fe"));

            migrationBuilder.AddColumn<string>(
                name: "Category",
                schema: "public",
                table: "ProfileRoles",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "public",
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("cd225915-7d46-4a10-9368-bcbbb6721b19"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEIigfHklevy86+yOgUlSTovndkhczdHUlX9aHb+l4YMhG3QTAgISeu8niRbpOKeKJg==");

            migrationBuilder.InsertData(
                schema: "public",
                table: "Companies",
                columns: new[] { "Id", "CreatedBy", "Description", "IsRegistry", "Name", "Status", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("0195ec3e-67a7-71f1-b89f-34d7f26ec2cf"), new Guid("00000000-0000-0000-0000-000000000000"), null, true, "Компания 2", "Approved", new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("0195ec3e-67a7-779b-986e-37af459f07cc"), new Guid("00000000-0000-0000-0000-000000000000"), null, true, "Компания 3", "Approved", new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("0195ec3e-67a7-78c6-962c-3f1455c0e89a"), new Guid("00000000-0000-0000-0000-000000000000"), null, true, "Компания 1", "Approved", new Guid("00000000-0000-0000-0000-000000000000") }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Companies",
                columns: new[] { "Id", "CreatedBy", "Description", "IsRegistry", "Name", "UpdatedBy" },
                values: new object[] { new Guid("0195ec3e-67a7-7ff8-bdec-116b62aa1fed"), new Guid("00000000-0000-0000-0000-000000000000"), null, true, "Компания 4", new Guid("00000000-0000-0000-0000-000000000000") });

            migrationBuilder.InsertData(
                schema: "public",
                table: "CompanyIndustries",
                columns: new[] { "Id", "CreatedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("1a22740c-a4e9-403e-bb61-1f0048ea1a58"), new DateTime(2025, 3, 31, 12, 47, 47, 633, DateTimeKind.Utc).AddTicks(9964), "Судостроение и морское строительство", new DateTime(2025, 3, 31, 12, 47, 47, 633, DateTimeKind.Utc).AddTicks(9965) },
                    { new Guid("295ee16d-f65c-48ca-a74b-0ff7bd3306dd"), new DateTime(2025, 3, 31, 12, 47, 47, 633, DateTimeKind.Utc).AddTicks(9942), "Архитектура и строительство", new DateTime(2025, 3, 31, 12, 47, 47, 633, DateTimeKind.Utc).AddTicks(9943) },
                    { new Guid("4d9da906-facf-493c-9aa3-83d8af57b8af"), new DateTime(2025, 3, 31, 12, 47, 47, 633, DateTimeKind.Utc).AddTicks(9947), "Электроника и высокие технологии", new DateTime(2025, 3, 31, 12, 47, 47, 633, DateTimeKind.Utc).AddTicks(9947) },
                    { new Guid("9a5e9600-d215-4933-8031-4eb4765dcc83"), new DateTime(2025, 3, 31, 12, 47, 47, 633, DateTimeKind.Utc).AddTicks(9958), "Промышленное оборудование", new DateTime(2025, 3, 31, 12, 47, 47, 633, DateTimeKind.Utc).AddTicks(9958) },
                    { new Guid("c91db8bf-c132-47a1-bd62-93727ebfec80"), new DateTime(2025, 3, 31, 12, 47, 47, 633, DateTimeKind.Utc).AddTicks(9945), "Аэрокосмическая отрасль", new DateTime(2025, 3, 31, 12, 47, 47, 633, DateTimeKind.Utc).AddTicks(9945) },
                    { new Guid("d2a01819-ed43-41d2-b12f-d859acbeb09e"), new DateTime(2025, 3, 31, 12, 47, 47, 633, DateTimeKind.Utc).AddTicks(9956), "Добыча полезных ископаемых", new DateTime(2025, 3, 31, 12, 47, 47, 633, DateTimeKind.Utc).AddTicks(9956) },
                    { new Guid("d5d8d844-b043-479c-8489-4d891952d149"), new DateTime(2025, 3, 31, 12, 47, 47, 633, DateTimeKind.Utc).AddTicks(9954), "Энергетика", new DateTime(2025, 3, 31, 12, 47, 47, 633, DateTimeKind.Utc).AddTicks(9954) },
                    { new Guid("dc182bf7-9886-499a-b48f-982ddeca2175"), new DateTime(2025, 3, 31, 12, 47, 47, 633, DateTimeKind.Utc).AddTicks(9348), "Автомобилестроение и транспорт", new DateTime(2025, 3, 31, 12, 47, 47, 633, DateTimeKind.Utc).AddTicks(9575) },
                    { new Guid("faecc456-9940-4ef8-acf4-12df0fd343bc"), new DateTime(2025, 3, 31, 12, 47, 47, 633, DateTimeKind.Utc).AddTicks(9949), "Инфраструктура", new DateTime(2025, 3, 31, 12, 47, 47, 633, DateTimeKind.Utc).AddTicks(9949) }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "ProfileRoles",
                columns: new[] { "Id", "Category", "CreatedAt", "Key", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0165559f-56cc-47b4-9810-23759eba3df2"), "Проектирование", new DateTime(2025, 3, 31, 12, 47, 56, 640, DateTimeKind.Utc).AddTicks(6370), "Estimator", "Инженер-сметчик", new DateTime(2025, 3, 31, 12, 47, 56, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("0ffe4fc6-a0f2-4a08-89ea-8cde7922145e"), "Эксплуатация", new DateTime(2025, 3, 31, 12, 48, 10, 640, DateTimeKind.Utc).AddTicks(6370), "ChiefTechnologist", "Главный технолог", new DateTime(2025, 3, 31, 12, 48, 10, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("1dc94fe3-11c8-48cf-8c1b-827b2d47ffe8"), "Эксплуатация", new DateTime(2025, 3, 31, 12, 48, 9, 640, DateTimeKind.Utc).AddTicks(6370), "ChiefPowerEngineer", "Главный энергетик", new DateTime(2025, 3, 31, 12, 48, 9, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("4197a34f-7ea3-4788-95c4-61b20360f068"), "Строительство", new DateTime(2025, 3, 31, 12, 48, 1, 640, DateTimeKind.Utc).AddTicks(6370), "ProcurementManager", "Менеджер по закупкам", new DateTime(2025, 3, 31, 12, 48, 1, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("499402ff-3f16-4537-8759-8e6cee73996f"), "Эксплуатация", new DateTime(2025, 3, 31, 12, 48, 7, 640, DateTimeKind.Utc).AddTicks(6370), "MainEngineer", "Главный инженер", new DateTime(2025, 3, 31, 12, 48, 7, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("51eb7a6b-2223-4966-b2a8-ae600a12169e"), "Проектирование", new DateTime(2025, 3, 31, 12, 47, 51, 640, DateTimeKind.Utc).AddTicks(6370), "Designer", "Дизайнер", new DateTime(2025, 3, 31, 12, 47, 51, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("57771399-25fb-478e-94f3-89975c91f344"), "Изыскания", new DateTime(2025, 3, 31, 12, 47, 48, 640, DateTimeKind.Utc).AddTicks(6370), "Topographer", "Топограф", new DateTime(2025, 3, 31, 12, 47, 48, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("5b59d996-0f34-40d2-aa27-92d32e95d78f"), "Управление", new DateTime(2025, 3, 31, 12, 48, 16, 640, DateTimeKind.Utc).AddTicks(6370), "Other", "Другое", new DateTime(2025, 3, 31, 12, 48, 16, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("5d156208-c54a-44cc-8d57-83cddf25d47e"), "Проектирование", new DateTime(2025, 3, 31, 12, 47, 58, 640, DateTimeKind.Utc).AddTicks(6370), "ChiefEngineer", "Главный инженер проекта", new DateTime(2025, 3, 31, 12, 47, 58, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("6e3e29a7-1185-4421-b47d-382e95576c89"), "Проектирование", new DateTime(2025, 3, 31, 12, 47, 54, 640, DateTimeKind.Utc).AddTicks(6370), "Infrastructure", "Специалист инженерного обеспечения территорий", new DateTime(2025, 3, 31, 12, 47, 54, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("6f2bf8ea-7035-428e-a943-80827b9ee139"), "Управление", new DateTime(2025, 3, 31, 12, 48, 15, 640, DateTimeKind.Utc).AddTicks(6370), "ProjectManager", "Руководитель проекта", new DateTime(2025, 3, 31, 12, 48, 15, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("7972e6c7-4ccb-4af0-8d60-081cd4ab0b6d"), "Проектирование", new DateTime(2025, 3, 31, 12, 47, 55, 640, DateTimeKind.Utc).AddTicks(6370), "EngineerDesigner", "Инженер-проектировщик", new DateTime(2025, 3, 31, 12, 47, 55, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("7e209785-5328-4e98-bdeb-91a224cf906c"), "Проектирование", new DateTime(2025, 3, 31, 12, 47, 59, 640, DateTimeKind.Utc).AddTicks(6370), "ProcessEngineer", "Инженер-технолог", new DateTime(2025, 3, 31, 12, 47, 59, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("8bd90b3b-f3b3-4282-999d-94e724650fd9"), "Управление", new DateTime(2025, 3, 31, 12, 48, 12, 640, DateTimeKind.Utc).AddTicks(6370), "GenDirector", "Генеральный директор", new DateTime(2025, 3, 31, 12, 48, 12, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("8d73af1b-83eb-4315-982b-3364558883f5"), "Строительство", new DateTime(2025, 3, 31, 12, 48, 6, 640, DateTimeKind.Utc).AddTicks(6370), "ConstructionControl", "Инженер по строительному контролю", new DateTime(2025, 3, 31, 12, 48, 6, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("91fc83f7-cd5b-4240-ad76-c8a5839e9125"), "Строительство", new DateTime(2025, 3, 31, 12, 48, 5, 640, DateTimeKind.Utc).AddTicks(6370), "QualityEngineer", "Инженер по качеству", new DateTime(2025, 3, 31, 12, 48, 5, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("9775953e-a11a-4a1e-bb40-180296393f99"), "Управление", new DateTime(2025, 3, 31, 12, 48, 13, 640, DateTimeKind.Utc).AddTicks(6370), "Technical", "Технический директор", new DateTime(2025, 3, 31, 12, 48, 13, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("9ece61f7-6c91-4ada-8b31-e841211b91ad"), "Проектирование", new DateTime(2025, 3, 31, 12, 47, 49, 640, DateTimeKind.Utc).AddTicks(6370), "Arch", "Архитектор", new DateTime(2025, 3, 31, 12, 47, 49, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("b3f77dea-4638-4292-a2f4-34036e9c0241"), "Проектирование", new DateTime(2025, 3, 31, 12, 47, 57, 640, DateTimeKind.Utc).AddTicks(6370), "StructuralEngineer", "Инженер-конструктор", new DateTime(2025, 3, 31, 12, 47, 57, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("ba25fc41-8acc-4b88-aa30-44b7d4d65993"), "Строительство", new DateTime(2025, 3, 31, 12, 48, 2, 640, DateTimeKind.Utc).AddTicks(6370), "Mechanic", "Механик", new DateTime(2025, 3, 31, 12, 48, 2, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("ba769055-70f0-44d2-93b2-bb92c9ddebb3"), "Изыскания", new DateTime(2025, 3, 31, 12, 47, 47, 640, DateTimeKind.Utc).AddTicks(6370), "SurveySpecialist", "Специалист по инженерным изысканиям", new DateTime(2025, 3, 31, 12, 47, 47, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("bea130b5-1b0b-4b91-817d-bf82b73e48d8"), "Проектирование", new DateTime(2025, 3, 31, 12, 48, 0, 640, DateTimeKind.Utc).AddTicks(6370), "BIMManager", "Менеджер ТИМ", new DateTime(2025, 3, 31, 12, 48, 0, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("c14c4b30-26f7-490e-8c93-937edf1f1580"), "Эксплуатация", new DateTime(2025, 3, 31, 12, 48, 8, 640, DateTimeKind.Utc).AddTicks(6370), "ChiefMechanic", "Главный механик", new DateTime(2025, 3, 31, 12, 48, 8, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("d1ee8666-1496-449b-b807-513cb6ab3ca3"), "Проектирование", new DateTime(2025, 3, 31, 12, 47, 50, 640, DateTimeKind.Utc).AddTicks(6370), "LandscapeArch", "Ландшафтный архитектор", new DateTime(2025, 3, 31, 12, 47, 50, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("da2d796a-1320-4741-9fe6-16f1b0d3fbb2"), "Проектирование", new DateTime(2025, 3, 31, 12, 47, 53, 640, DateTimeKind.Utc).AddTicks(6370), "TransportDev", "Специалист транспортного развития территорий", new DateTime(2025, 3, 31, 12, 47, 53, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("ddc4a5bb-c0f5-468f-9c4a-a48240957d99"), "Строительство", new DateTime(2025, 3, 31, 12, 48, 3, 640, DateTimeKind.Utc).AddTicks(6370), "Builder", "Строитель", new DateTime(2025, 3, 31, 12, 48, 3, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("eb0538e9-2536-43f0-8a2a-7dab399b5b06"), "Эксплуатация", new DateTime(2025, 3, 31, 12, 48, 11, 640, DateTimeKind.Utc).AddTicks(6370), "OperationsEngineer", "Инженер по эксплуатации", new DateTime(2025, 3, 31, 12, 48, 11, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("f4ab5641-d9ec-4bd4-a95b-7a0644861008"), "Проектирование", new DateTime(2025, 3, 31, 12, 47, 52, 640, DateTimeKind.Utc).AddTicks(6370), "ChiefArch", "Главный архитектор проекта", new DateTime(2025, 3, 31, 12, 47, 52, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("f8acba79-1675-4ff8-bef7-97a6608c745f"), "Строительство", new DateTime(2025, 3, 31, 12, 48, 4, 640, DateTimeKind.Utc).AddTicks(6370), "Foreman", "Прораб", new DateTime(2025, 3, 31, 12, 48, 4, 640, DateTimeKind.Utc).AddTicks(6370) },
                    { new Guid("fc153547-5fb7-450d-9d28-68c814cd0be7"), "Управление", new DateTime(2025, 3, 31, 12, 48, 14, 640, DateTimeKind.Utc).AddTicks(6370), "Financial", "Финансовый директор", new DateTime(2025, 3, 31, 12, 48, 14, 640, DateTimeKind.Utc).AddTicks(6370) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "public",
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("0195ec3e-67a7-71f1-b89f-34d7f26ec2cf"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("0195ec3e-67a7-779b-986e-37af459f07cc"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("0195ec3e-67a7-78c6-962c-3f1455c0e89a"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("0195ec3e-67a7-7ff8-bdec-116b62aa1fed"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "CompanyIndustries",
                keyColumn: "Id",
                keyValue: new Guid("1a22740c-a4e9-403e-bb61-1f0048ea1a58"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "CompanyIndustries",
                keyColumn: "Id",
                keyValue: new Guid("295ee16d-f65c-48ca-a74b-0ff7bd3306dd"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "CompanyIndustries",
                keyColumn: "Id",
                keyValue: new Guid("4d9da906-facf-493c-9aa3-83d8af57b8af"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "CompanyIndustries",
                keyColumn: "Id",
                keyValue: new Guid("9a5e9600-d215-4933-8031-4eb4765dcc83"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "CompanyIndustries",
                keyColumn: "Id",
                keyValue: new Guid("c91db8bf-c132-47a1-bd62-93727ebfec80"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "CompanyIndustries",
                keyColumn: "Id",
                keyValue: new Guid("d2a01819-ed43-41d2-b12f-d859acbeb09e"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "CompanyIndustries",
                keyColumn: "Id",
                keyValue: new Guid("d5d8d844-b043-479c-8489-4d891952d149"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "CompanyIndustries",
                keyColumn: "Id",
                keyValue: new Guid("dc182bf7-9886-499a-b48f-982ddeca2175"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "CompanyIndustries",
                keyColumn: "Id",
                keyValue: new Guid("faecc456-9940-4ef8-acf4-12df0fd343bc"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0165559f-56cc-47b4-9810-23759eba3df2"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0ffe4fc6-a0f2-4a08-89ea-8cde7922145e"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("1dc94fe3-11c8-48cf-8c1b-827b2d47ffe8"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("4197a34f-7ea3-4788-95c4-61b20360f068"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("499402ff-3f16-4537-8759-8e6cee73996f"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("51eb7a6b-2223-4966-b2a8-ae600a12169e"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("57771399-25fb-478e-94f3-89975c91f344"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("5b59d996-0f34-40d2-aa27-92d32e95d78f"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("5d156208-c54a-44cc-8d57-83cddf25d47e"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("6e3e29a7-1185-4421-b47d-382e95576c89"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("6f2bf8ea-7035-428e-a943-80827b9ee139"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("7972e6c7-4ccb-4af0-8d60-081cd4ab0b6d"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("7e209785-5328-4e98-bdeb-91a224cf906c"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("8bd90b3b-f3b3-4282-999d-94e724650fd9"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d73af1b-83eb-4315-982b-3364558883f5"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("91fc83f7-cd5b-4240-ad76-c8a5839e9125"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("9775953e-a11a-4a1e-bb40-180296393f99"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("9ece61f7-6c91-4ada-8b31-e841211b91ad"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("b3f77dea-4638-4292-a2f4-34036e9c0241"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("ba25fc41-8acc-4b88-aa30-44b7d4d65993"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("ba769055-70f0-44d2-93b2-bb92c9ddebb3"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("bea130b5-1b0b-4b91-817d-bf82b73e48d8"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("c14c4b30-26f7-490e-8c93-937edf1f1580"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("d1ee8666-1496-449b-b807-513cb6ab3ca3"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("da2d796a-1320-4741-9fe6-16f1b0d3fbb2"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("ddc4a5bb-c0f5-468f-9c4a-a48240957d99"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("eb0538e9-2536-43f0-8a2a-7dab399b5b06"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("f4ab5641-d9ec-4bd4-a95b-7a0644861008"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("f8acba79-1675-4ff8-bef7-97a6608c745f"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("fc153547-5fb7-450d-9d28-68c814cd0be7"));

            migrationBuilder.DropColumn(
                name: "Category",
                schema: "public",
                table: "ProfileRoles");

            migrationBuilder.UpdateData(
                schema: "public",
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("cd225915-7d46-4a10-9368-bcbbb6721b19"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEOCvfWwoUWPzeKCQVoWIfPTyO8Q7Q5F7cA91VlW510kTX5jr1C/BF57zYoS2O5s1ug==");

            migrationBuilder.InsertData(
                schema: "public",
                table: "Companies",
                columns: new[] { "Id", "CreatedBy", "Description", "IsRegistry", "Name", "Status", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("0195c7e3-c5c9-7244-bd6b-3c6287a7fc9c"), new Guid("00000000-0000-0000-0000-000000000000"), null, true, "Компания 2", "Approved", new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("0195c7e3-c5c9-7265-8880-83378a36ea65"), new Guid("00000000-0000-0000-0000-000000000000"), null, true, "Компания 1", "Approved", new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("0195c7e3-c5c9-730b-86f5-e32838a3db98"), new Guid("00000000-0000-0000-0000-000000000000"), null, true, "Компания 3", "Approved", new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("0195c7e3-c5c9-7ec5-9c53-0cb4c038c85e"), new Guid("00000000-0000-0000-0000-000000000000"), null, true, "Компания 4", "Init", new Guid("00000000-0000-0000-0000-000000000000") }
                });

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
        }
    }
}
