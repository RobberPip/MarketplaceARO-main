using RegistryApp.Domain.Models;
using Npgsql;

namespace RegistryApp.DB;

public partial class PostgreSqlAdapter : IDisposable {
    /// <summary>
    /// Получить все атрибуты
    /// </summary>
    public async Task<List<Attributes>> GetAllAttributesAsync() {
        try {
            List<Attributes> attributesList = new List<Attributes>();
            await connection.OpenAsync();
            string query = $"SET search_path = {dbSchema}; SELECT id, attribute_name, filterable_attribute, " +
                           $"(SELECT scope FROM guide_scope_attribute WHERE guide_attribute.id_scope=guide_scope_attribute.id)," +
                           $"(SELECT value FROM attribute_possible_values WHERE guide_attribute.id = attribute_possible_values.attribute_id) " +
                           $"FROM guide_attribute";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection)) {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        long id = reader.GetInt64(0);
                        string? name = reader.GetString(1);
                        bool filterable = reader.GetBoolean(2);
                        string scope = reader.GetString(3);
                        string[] attributeValueArray = reader.IsDBNull(4) ? null : (string[])reader.GetValue(4);

                        Attributes attribute = new Attributes(id, name, filterable, scope,
                            attributeValueArray != null ? new List<string>(attributeValueArray) : null);
                        attributesList.Add(attribute);
                    }
                }
            }
            await connection.CloseAsync();
            return attributesList;
        } catch (Exception ex) {
            Console.WriteLine($"Detailed description of the error: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Получить области действия атрибутов, обернутые внутри класса
    /// </summary>
    public async Task<Attributes> GetAllScopeAttributesAsync() {
        try {
            Attributes attributes = new Attributes();
            await connection.OpenAsync();
            string query = $"SET search_path = {dbSchema}; SELECT scope FROM guide_scope_attribute";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection)) {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        if (!reader.IsDBNull(0)) {
                            attributes.AllScope.Add(reader.GetString(0));
                        }
                    }
                }
            }
            await connection.CloseAsync();
            return attributes;
        } catch (Exception ex) {
            Console.WriteLine($"Detailed description of the error: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Получить все возможные значения атрибута по его имени
    /// </summary>
    /// <param name="attributeName">Имя атрибута</param>
    public async Task<string[]> GetByNameAttributesPossibleValuesAsync(string attributeName) {
        try {
            List<string> attributeValues = new List<string>();
            await connection.OpenAsync();
            string query =
                $"SET search_path = {dbSchema}; SELECT attribute_possible_values.value " +
                $"FROM guide_attribute " +
                $"LEFT JOIN attribute_possible_values ON guide_attribute.id = attribute_possible_values.attribute_id " +
                $"WHERE attribute_possible_values.value IS NOT NULL " +
                $"AND guide_attribute.attribute_name LIKE '{attributeName}'";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection)) {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        if (!reader.IsDBNull(0)) {
                            string[] attributeValueArray = reader.GetFieldValue<string[]>(0);
                            attributeValues.AddRange(attributeValueArray);
                        }
                    }
                }
            }
            await connection.CloseAsync();
            return attributeValues.ToArray();
        } catch (Exception ex) {
            Console.WriteLine($"Detailed description of the error: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Получить атрибут по id
    /// </summary>
    /// <param name="_id">Id атрибута</param>
    public async Task<Attributes> GetAttributeByIdAsync(long _id) {
        try {
            Attributes attribute = new Attributes();
            await connection.OpenAsync();
            string query =
                @$"SET search_path = {dbSchema}; SELECT id, attribute_name, filterable_attribute,
                (SELECT scope FROM guide_scope_attribute WHERE guide_attribute.id_scope = guide_scope_attribute.id),
                (SELECT value FROM attribute_possible_values WHERE guide_attribute.id = attribute_possible_values.attribute_id),
                (SELECT COUNT(*) FROM value_attribute WHERE value_attribute.attribute_id = guide_attribute.id)
                FROM guide_attribute WHERE id = '{_id}'";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection)) {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        if (!reader.IsDBNull(0)) {
                            long id = reader.GetInt64(0);
                            string? name = reader.GetString(1);
                            bool filterable = reader.GetBoolean(2);
                            string scope = reader.GetString(3);
                            string[] attributeValueArray = reader.IsDBNull(4) ? null : (string[])reader.GetValue(4);

                            attribute = new Attributes(id, name, filterable, scope, attributeValueArray != null ? new List<string>(attributeValueArray) : null);
                            attribute.DependentCount = reader.GetInt64(5);
                        }
                    }
                }
            }
            await connection.CloseAsync();
            return attribute;
        } catch (Exception ex) {
            Console.WriteLine($"Detailed description of the error: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Положить атрибут в базу данных
    /// </summary>
    /// <param name="attribute">Объект атрибута</param>
    public async Task AddAttributeAsync(Attributes attribute) {
        try {
            await connection.OpenAsync();
            using (var command = new NpgsqlCommand($"CALL {dbSchema}.\"AddAttribute\" (@name,@filterable,@scope,@possible_values)", connection)) {
                command.Parameters.AddWithValue("@name", attribute.Name!);
                command.Parameters.AddWithValue("@filterable", attribute.FilterableAttribute!);
                command.Parameters.AddWithValue("@scope", attribute.Scope! != null ? (object)attribute.Scope : DBNull.Value);
                command.Parameters.AddWithValue("@possible_values", attribute.AttributesPossibleValues! != null ? attribute.AttributesPossibleValues.ToArray() : DBNull.Value);
                await command.ExecuteNonQueryAsync();
            }
            await connection.CloseAsync();
        } catch (Exception ex) {
            Console.WriteLine($"Detailed description of the error: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Обновить атрибут в базе данных
    /// </summary>
    /// <param name="attribute">Объект атрибута</param>
    public async Task UpdateAttributeAsync(Attributes attribute) {
        try {
            await connection.OpenAsync();
            using (var command = new NpgsqlCommand($"CALL {dbSchema}.\"UpdateAttribute\" (@id,@name,@filterable,@scope,@possible_values)", connection)) {
                command.Parameters.AddWithValue("@id", attribute.Id);
                command.Parameters.AddWithValue("@name", attribute.Name);
                command.Parameters.AddWithValue("@filterable", attribute.FilterableAttribute!);
                command.Parameters.AddWithValue("@scope", attribute.Scope! != null ? (object)attribute.Scope : DBNull.Value);
                command.Parameters.AddWithValue("@possible_values", attribute.AttributesPossibleValues! != null ? attribute.AttributesPossibleValues.ToArray() : DBNull.Value);
                await command.ExecuteNonQueryAsync();
            }
            await connection.CloseAsync();
        } catch (Exception ex) {
            Console.WriteLine($"Detailed description of the error: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Удалить атрибут, если у него нет зависимостей
    /// </summary>
    /// <param name="id">Id атрибута</param>
    public async Task DeleteAttributeByIdAsync(long id) {
        try {
            await connection.OpenAsync();
            using (NpgsqlCommand command = new NpgsqlCommand($"CALL {dbSchema}.\"DeleteAttributeById\" (@id)", connection)) {
                command.Parameters.AddWithValue("@id", id);
                await command.ExecuteNonQueryAsync();
            }
            await connection.CloseAsync();
        } catch (Exception ex) {
            Console.WriteLine($"Detailed description of the error: {ex}");
            throw;
        }
    }
}