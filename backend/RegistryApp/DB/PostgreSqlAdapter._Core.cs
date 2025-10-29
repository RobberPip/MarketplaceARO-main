using Npgsql;
using System.Text;

namespace RegistryApp.DB;

public partial class PostgreSqlAdapter : IDisposable {
    private NpgsqlConnection? connection;
    private static string? dbSchema;

    public PostgreSqlAdapter(IConfiguration config) {
        var connectionString = config.GetConnectionString("Default") ??
            throw new ArgumentException("Connection string not found in config");

        connection = new NpgsqlConnection(connectionString);
        dbSchema = config.GetValue<string>("DbSchema");
        dbSchema = string.IsNullOrWhiteSpace(dbSchema) ? "public" : dbSchema;

    }

    public void Dispose() {
        DisposeAsync().GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync() {
        if (connection != null) {
            await connection.DisposeAsync();
            connection = null;
        }
    }
}
