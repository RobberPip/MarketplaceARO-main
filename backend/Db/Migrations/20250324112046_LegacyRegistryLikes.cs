using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Db.Migrations {
    /// <inheritdoc />
    public partial class LegacyRegistryLikes : Migration {
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

            ExecuteMigration("Db.Migrations.Scripts.LegacyRegistryLikes.sql");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {

        }
    }
}
