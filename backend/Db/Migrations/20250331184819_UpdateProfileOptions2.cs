using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Db.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProfileOptions2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "CompanyIndustries",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.UpdateData(
                schema: "public",
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("cd225915-7d46-4a10-9368-bcbbb6721b19"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEH5YPaFaXdGDfnXARoUD5GC7kZAKuDJENuaSmuRqJl5r1AA1V0TVimDDXCRl2nSdlQ==");

            migrationBuilder.InsertData(
                schema: "public",
                table: "Companies",
                columns: new[] { "Id", "CreatedBy", "Description", "IsRegistry", "Name", "Status", "UpdatedBy" },
                values: new object[] { new Guid("0195ed88-7798-731e-aa91-bd895831d92e"), new Guid("00000000-0000-0000-0000-000000000000"), null, true, "Компания 3", "Approved", new Guid("00000000-0000-0000-0000-000000000000") });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Companies",
                columns: new[] { "Id", "CreatedBy", "Description", "IsRegistry", "Name", "UpdatedBy" },
                values: new object[] { new Guid("0195ed88-7798-75ea-8584-5da572f9a27a"), new Guid("00000000-0000-0000-0000-000000000000"), null, true, "Компания 4", new Guid("00000000-0000-0000-0000-000000000000") });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Companies",
                columns: new[] { "Id", "CreatedBy", "Description", "IsRegistry", "Name", "Status", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("0195ed88-7798-7b61-8b7d-905deb18f5f4"), new Guid("00000000-0000-0000-0000-000000000000"), null, true, "Компания 1", "Approved", new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("0195ed88-7798-7e60-b20e-d72597217cab"), new Guid("00000000-0000-0000-0000-000000000000"), null, true, "Компания 2", "Approved", new Guid("00000000-0000-0000-0000-000000000000") }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "CompanyIndustries",
                columns: new[] { "Id", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0194f4e0-4642-48d0-8324-eced7a30b57d"), "Архитектура и строительство", null },
                    { new Guid("0b49d123-65dc-4bfb-80d1-48c14136439b"), "Автомобилестроение и транспорт", null },
                    { new Guid("a32f0d82-6e6a-4be7-892e-9f2db3848c8c"), "Инфраструктура", null },
                    { new Guid("b12961ae-ecdf-48be-b439-c6406b76aa13"), "Электроника и высокие технологии", null },
                    { new Guid("deaa02b1-fe0d-4a32-9af4-7038bc10d715"), "Судостроение и морское строительство", null },
                    { new Guid("e429e4a1-a3be-4047-a33a-06da9181b15d"), "Добыча полезных ископаемых", null },
                    { new Guid("ef366898-bb7e-4c87-adb4-a112cb3902b8"), "Аэрокосмическая отрасль", null },
                    { new Guid("f04fbaea-a656-420c-a52c-dcedd6fabe75"), "Энергетика", null },
                    { new Guid("fe4f5bcd-c207-4c51-9fa2-6c0d424debad"), "Промышленное оборудование", null }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "ProfileRoles",
                columns: new[] { "Id", "Category", "Key", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0195ed88-77a2-7c1c-888b-f086435849e5"), "Изыскания", "SurveySpecialist", "Специалист по инженерным изысканиям", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-7b8a-7174-bc49-5a108e98b622"), "Изыскания", "Topographer", "Топограф", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-7f72-769a-9b61-7b132cc524a7"), "Проектирование", "Arch", "Архитектор", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-835a-7198-a1a2-60c22c412b8d"), "Проектирование", "LandscapeArch", "Ландшафтный архитектор", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-8742-7fd0-8e02-44e2ee843fdb"), "Проектирование", "Designer", "Дизайнер", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-8b2a-7a95-9595-ea6833ac14bf"), "Проектирование", "ChiefArch", "Главный архитектор проекта", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-8f12-72c5-b23c-b2b35d0b6428"), "Проектирование", "TransportDev", "Специалист транспортного развития территорий", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-92fa-77bb-9f41-488e8fa59e4e"), "Проектирование", "Infrastructure", "Специалист инженерного обеспечения территорий", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-96e2-7d79-b292-79c2b0f885ac"), "Проектирование", "EngineerDesigner", "Инженер-проектировщик", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-9aca-71eb-9867-e93e4390263f"), "Проектирование", "Estimator", "Инженер-сметчик", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-9eb2-7cf9-b1a5-e8b281474259"), "Проектирование", "StructuralEngineer", "Инженер-конструктор", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-a29a-776c-8b63-6983e96b2b1e"), "Проектирование", "ChiefEngineer", "Главный инженер проекта", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-a682-713d-949a-c3b90ceb4f09"), "Проектирование", "ProcessEngineer", "Инженер-технолог", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-aa6a-7140-8f89-39f15b083570"), "Проектирование", "BIMManager", "Менеджер ТИМ", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-ae52-7880-a5fc-fb9092c2f438"), "Строительство", "ProcurementManager", "Менеджер по закупкам", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-b23a-70f1-9bcc-a4f572740702"), "Строительство", "Mechanic", "Механик", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-b622-7694-a12c-743e14ed45c1"), "Строительство", "Builder", "Строитель", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-ba0a-7dcf-b555-d882b4466df8"), "Строительство", "Foreman", "Прораб", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-bdf2-7301-a376-37b340ce71c6"), "Строительство", "QualityEngineer", "Инженер по качеству", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-c1da-7a33-84b5-e1264180730d"), "Строительство", "ConstructionControl", "Инженер по строительному контролю", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-c5c2-789c-8b05-bf0529dc62c1"), "Эксплуатация", "MainEngineer", "Главный инженер", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-c9aa-76f7-9e36-9e52a3a9d396"), "Эксплуатация", "ChiefMechanic", "Главный механик", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-cd92-75fd-bec9-cdbfd7694d2d"), "Эксплуатация", "ChiefPowerEngineer", "Главный энергетик", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-d17a-7969-8581-f234737eb351"), "Эксплуатация", "ChiefTechnologist", "Главный технолог", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-d562-743a-9f06-fa974f06bb24"), "Эксплуатация", "OperationsEngineer", "Инженер по эксплуатации", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-d94a-738f-b015-2c884b3766a9"), "Управление", "GenDirector", "Генеральный директор", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-dd32-7e25-8c6e-c69f40386f19"), "Управление", "Technical", "Технический директор", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-e11a-7713-a8e8-1553d323ee18"), "Управление", "Financial", "Финансовый директор", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-e502-7d45-9a49-a75d571d36e5"), "Управление", "ProjectManager", "Руководитель проекта", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0195ed88-e8ea-7693-b851-f35124e98fe9"), "Управление", "Other", "Другое", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "public",
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-7798-731e-aa91-bd895831d92e"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-7798-75ea-8584-5da572f9a27a"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-7798-7b61-8b7d-905deb18f5f4"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-7798-7e60-b20e-d72597217cab"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "CompanyIndustries",
                keyColumn: "Id",
                keyValue: new Guid("0194f4e0-4642-48d0-8324-eced7a30b57d"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "CompanyIndustries",
                keyColumn: "Id",
                keyValue: new Guid("0b49d123-65dc-4bfb-80d1-48c14136439b"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "CompanyIndustries",
                keyColumn: "Id",
                keyValue: new Guid("a32f0d82-6e6a-4be7-892e-9f2db3848c8c"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "CompanyIndustries",
                keyColumn: "Id",
                keyValue: new Guid("b12961ae-ecdf-48be-b439-c6406b76aa13"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "CompanyIndustries",
                keyColumn: "Id",
                keyValue: new Guid("deaa02b1-fe0d-4a32-9af4-7038bc10d715"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "CompanyIndustries",
                keyColumn: "Id",
                keyValue: new Guid("e429e4a1-a3be-4047-a33a-06da9181b15d"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "CompanyIndustries",
                keyColumn: "Id",
                keyValue: new Guid("ef366898-bb7e-4c87-adb4-a112cb3902b8"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "CompanyIndustries",
                keyColumn: "Id",
                keyValue: new Guid("f04fbaea-a656-420c-a52c-dcedd6fabe75"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "CompanyIndustries",
                keyColumn: "Id",
                keyValue: new Guid("fe4f5bcd-c207-4c51-9fa2-6c0d424debad"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-77a2-7c1c-888b-f086435849e5"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-7b8a-7174-bc49-5a108e98b622"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-7f72-769a-9b61-7b132cc524a7"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-835a-7198-a1a2-60c22c412b8d"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-8742-7fd0-8e02-44e2ee843fdb"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-8b2a-7a95-9595-ea6833ac14bf"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-8f12-72c5-b23c-b2b35d0b6428"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-92fa-77bb-9f41-488e8fa59e4e"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-96e2-7d79-b292-79c2b0f885ac"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-9aca-71eb-9867-e93e4390263f"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-9eb2-7cf9-b1a5-e8b281474259"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-a29a-776c-8b63-6983e96b2b1e"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-a682-713d-949a-c3b90ceb4f09"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-aa6a-7140-8f89-39f15b083570"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-ae52-7880-a5fc-fb9092c2f438"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-b23a-70f1-9bcc-a4f572740702"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-b622-7694-a12c-743e14ed45c1"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-ba0a-7dcf-b555-d882b4466df8"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-bdf2-7301-a376-37b340ce71c6"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-c1da-7a33-84b5-e1264180730d"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-c5c2-789c-8b05-bf0529dc62c1"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-c9aa-76f7-9e36-9e52a3a9d396"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-cd92-75fd-bec9-cdbfd7694d2d"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-d17a-7969-8581-f234737eb351"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-d562-743a-9f06-fa974f06bb24"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-d94a-738f-b015-2c884b3766a9"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-dd32-7e25-8c6e-c69f40386f19"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-e11a-7713-a8e8-1553d323ee18"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-e502-7d45-9a49-a75d571d36e5"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ProfileRoles",
                keyColumn: "Id",
                keyValue: new Guid("0195ed88-e8ea-7693-b851-f35124e98fe9"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "CompanyIndustries",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

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
                    { new Guid("0195ec3e-67a7-78c6-962c-3f1455c0e89a"), new Guid("00000000-0000-0000-0000-000000000000"), null, true, "Компания 1", "Approved", new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("0195ec3e-67a7-7ff8-bdec-116b62aa1fed"), new Guid("00000000-0000-0000-0000-000000000000"), null, true, "Компания 4", "Init", new Guid("00000000-0000-0000-0000-000000000000") }
                });

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
    }
}
