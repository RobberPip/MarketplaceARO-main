using RegistryApp.Domain.Enums;
using RegistryApp.Domain.Models;
using Npgsql;

namespace RegistryApp.DB;

public partial class PostgreSqlAdapter : IDisposable {
    /// <summary>
    /// Получить всех пользователей
    /// </summary>
    public async Task<List<User>> GetAllUsersAsync() {
        try {
            List<User> usersList = new List<User>();
            await connection.OpenAsync();
            string query = $"SET search_path = {dbSchema}; SELECT users.id, name, email, password, role_id " +
                           $"FROM users JOIN role ON users.role_id=role.id";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection)) {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        long id = reader.GetInt64(0);
                        string? name = reader.GetString(1);
                        string? email = reader.IsDBNull(2) ? null : reader.GetString(2);
                        string? password = reader.IsDBNull(3) ? null : reader.GetString(3);
                        int roleId = reader.GetInt32(4);
                        Role _role = (Role)roleId;
                        User user = new User(id, name, email, password, _role);
                        usersList.Add(user);
                    }
                }
            }
            await connection.CloseAsync();
            return usersList;
        } catch (Exception ex) {
            Console.WriteLine($"Detailed description of the error: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Получить пользователя по id
    /// </summary>
    /// <param name="id">Id пользователя</param>
    public async Task<User> GetUserByIdAsync(long id) {
        try {
            User user = null;
            await connection.OpenAsync();
            var query =
                $"SET search_path = {dbSchema}; SELECT users.id, users.name, users.email, users.password, users.role_id FROM users " +
                $"JOIN role ON users.role_id = role.id WHERE users.id = @Id";

            using (var command = new NpgsqlCommand(query, connection)) {
                command.Parameters.AddWithValue("Id", id);
                using (var reader = await command.ExecuteReaderAsync()) {
                    if (await reader.ReadAsync()) {
                        id = reader.GetInt64(0);
                        string? Name = reader.GetString(1);
                        string? Email = reader.IsDBNull(2) ? null : reader.GetString(2);
                        string? Password = reader.IsDBNull(3) ? null : reader.GetString(3);
                        int roleId = reader.GetInt32(4);
                        Role _role = (Role)roleId;
                        user = new User(id, Name, Email, Password, _role);
                    }
                }
            }
            await connection.CloseAsync();
            return user;
        } catch (Exception ex) {
            Console.WriteLine($"Detailed description of the error: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Обновить данные пользователя
    /// </summary>
    /// <param name="user">Объект пользователя</param>
    public async Task UpdateDataUserAsync(User user) {
        try {
            await connection.OpenAsync();
            using (var cmd = new NpgsqlCommand($"CALL {dbSchema}.\"UpdateUser\" (@flag_email,@flag_password,@id,@role,@name,@email,@password)", connection)) {
                cmd.Parameters.AddWithValue("@id", user.Id);
                cmd.Parameters.AddWithValue("@name", user.Name != null ? (object)user.Name : DBNull.Value);
                cmd.Parameters.AddWithValue("@email", user.Email! != null ? (object)user.Email : DBNull.Value);
                cmd.Parameters.AddWithValue("@password", user.Password! != null ? (object)user.Password : DBNull.Value);
                cmd.Parameters.AddWithValue("@role", user.Role != null ? (int)user.Role : DBNull.Value);
                cmd.Parameters.AddWithValue("@flag_email", user.flagEmail);
                cmd.Parameters.AddWithValue("@flag_password", user.flagPassword);
                await cmd.ExecuteNonQueryAsync();
            }
            await connection.CloseAsync();
        } catch (Exception ex) {
            Console.WriteLine($"Detailed description of the error: {ex}");
            throw;
        }
    }
}
