using Npgsql;

namespace RegistryApp.DB {
    public partial class PostgreSqlAdapter : IDisposable {
        public async Task<int> GetFilterShortLinkUniqueId(int randomCode, string hashData, string json) {
            try {
                object numberUniqueCode = 0;
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand($"SET search_path to {dbSchema}; " +
                    $"SELECT try_add_filter_shortlink_data(@unique_numcode, @hash_json_data, @source_json)", connection)) {
                    command.Parameters.AddWithValue("@unique_numcode", NpgsqlTypes.NpgsqlDbType.Integer, randomCode);
                    command.Parameters.AddWithValue("@hash_json_data", NpgsqlTypes.NpgsqlDbType.Varchar, hashData);
                    command.Parameters.AddWithValue("@source_json", NpgsqlTypes.NpgsqlDbType.Json, json);

                    numberUniqueCode = await command.ExecuteScalarAsync();
                }
                await connection.CloseAsync();

                return (int)numberUniqueCode;
            } catch {
                throw;
            }
        }

        public async Task<string?> TryGetFilterStageByUniqueId(int uniqueId) {
            try {
                object jsonData;
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand($"SET search_path to {dbSchema}; " +
                    $"SELECT try_get_filter_shortlink_data(@unique_id)", connection)) {
                    command.Parameters.AddWithValue("@unique_id", NpgsqlTypes.NpgsqlDbType.Integer, uniqueId);
                    jsonData = await command.ExecuteScalarAsync();
                }
                await connection.CloseAsync();

                return jsonData as string;
            } catch {
                throw;
            }
        }
    }
}
