using RegistryApp.Domain.Models;
using Npgsql;

namespace RegistryApp.DB;

public partial class PostgreSqlAdapter : IDisposable {
    /// <summary>
    /// Получить всех вендоров
    /// </summary>
    public async Task<List<Vendor>> GetAllVendorsAsync() {
        try {
            List<Vendor> vendorList = new List<Vendor>();
            await connection.OpenAsync();
            var query =
                $"SET search_path = {dbSchema}; SELECT vendors.id,vendors.name,vendors.description,vendors.link_to, vendors.other_name, vendors.public_name, vendors.logo " +
                $"FROM vendors;";

            List<long> masid = new List<long>();
            Vendor vendor = new Vendor();
            using (var command = new NpgsqlCommand(query, connection)) {
                using (var reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        long Id = reader.GetInt64(0);
                        string? Name = reader.GetString(1);
                        string? Description = reader.IsDBNull(2) ? null : reader.GetString(2);
                        string? LinkTo = reader.IsDBNull(3) ? null : reader.GetString(3);
                        string? otherName = reader.IsDBNull(4) ? null : reader.GetString(4);
                        string? publicName = reader.IsDBNull(5) ? null : reader.GetString(5);
                        byte[]? Logo = reader.IsDBNull(6) ? null : (byte[])reader[6];
                        masid.Add(Id);
                        vendor = vendorList.FirstOrDefault(s => s.Id == Id);
                        vendor = new Vendor(Id, Name, Description, LinkTo, otherName, publicName, new Dictionary<string, object>(), Logo);
                        vendorList.Add(vendor);
                    }
                }
            }

            query = $@"SET search_path = {dbSchema}; SELECT attribute_name,value_attribute.value, vendor_id FROM value_attribute
                    JOIN vendors ON vendor_id = vendors.id
                    JOIN guide_attribute ON attribute_id = guide_attribute.id WHERE vendor_id IN (SELECT unnest(@masid));";

            using (var command = new NpgsqlCommand(query, connection)) {
                command.Parameters.AddWithValue("masid", masid);
                using (var reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        long vendorId = reader.GetInt64(2);
                        string NameAttribute = reader.GetString(0);
                        object[] ValueAttribute = (object[])reader.GetValue(1);
                        vendor = vendorList.FirstOrDefault(s => s.Id == vendorId);
                        if (vendor != null) {
                            if (!vendor.Attributes.ContainsKey(NameAttribute)) {
                                vendor.Attributes[NameAttribute] = ValueAttribute;
                            }
                        }
                    }
                }
            }
            await connection.CloseAsync();
            return vendorList;
        } catch (Exception ex) {
            Console.WriteLine($"Detailed description of the error: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Получить вендора по id
    /// </summary>
    /// <param name="id">Id вендора</param>
    public async Task<Vendor> GetVendorByIdAsync(long id) {
        try {
            Vendor vendor = new Vendor();
            await connection.OpenAsync();
            var query =
                $"SET search_path = {dbSchema}; SELECT vendors.id,vendors.name,vendors.description,vendors.link_to, vendors.other_name, vendors.public_name, vendors.logo " +
                $"FROM vendors " +
                "WHERE vendors.id = @Id";
            using (var command = new NpgsqlCommand(query, connection)) {
                command.Parameters.AddWithValue("Id", id);
                using (var reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        id = reader.GetInt64(0);
                        string? Name = reader.GetString(1);
                        string? Description = reader.IsDBNull(2) ? null : reader.GetString(2);
                        string? LinkTo = reader.IsDBNull(3) ? null : reader.GetString(3);
                        string? otherName = reader.IsDBNull(4) ? null : reader.GetString(4);
                        string? publicName = reader.IsDBNull(5) ? null : reader.GetString(5);
                        byte[]? Logo = reader.IsDBNull(6) ? null : (byte[])reader[6];
                        vendor = new Vendor(id, Name, Description, LinkTo, otherName, publicName, new Dictionary<string, object>(), Logo);
                    }
                }
            }

            query = $"SET search_path = {dbSchema}; SELECT attribute_name,value_attribute.value, vendor_id FROM value_attribute " +
                $"JOIN vendors ON vendor_id = vendors.id " +
                $"JOIN guide_attribute ON attribute_id = guide_attribute.id WHERE vendor_id = @Id;";

            using (var command = new NpgsqlCommand(query, connection)) {
                command.Parameters.AddWithValue("Id", id);
                using (var reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        string NameAttribute = reader.GetString(0);
                        object[] ValueAttribute = (object[])reader.GetValue(1);
                        if (vendor != null) {
                            if (!vendor.Attributes.ContainsKey(NameAttribute)) {
                                vendor.Attributes[NameAttribute] = ValueAttribute;
                            }
                        }
                    }
                }
            }
            await connection.CloseAsync();
            return vendor;
        } catch (Exception ex) {
            Console.WriteLine($"Detailed description of the error: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Добавить вендора в базу данных
    /// </summary>
    /// <param name="vendor">Объект вендора</param>
    public async Task AddVendorAsync(Vendor vendor) {
        try {
            await connection.OpenAsync();
            string[] keys = null;
            string[] dataValues = null;
            string[] Filterablekeys = null;
            string[] FilterableDataValues = null;
            List<string> dataTypeList = new List<string>();
            string[] dataType = null;
            Type valueType = null;
            string typeName = null;

            if (vendor.Attributes != null) {
                foreach (var kvp in vendor.Attributes) {
                    if (kvp.Value is List<object>) {
                        valueType = kvp.Value.GetType();
                        if (valueType.IsGenericType) {
                            Type genericType = valueType.GetGenericArguments()[0];
                            typeName = genericType.Name;
                        }
                    } else {
                        valueType = kvp.Value.GetType();
                        typeName = valueType.Name;
                    }
                    dataTypeList.Add(typeName);
                }
                dataType = dataTypeList.ToArray();
            }
            if (vendor.Attributes != null) {
                keys = vendor.Attributes.Keys.ToArray();
                dataValues = vendor.Attributes.Values.Select(value => {
                    if (value is List<object> list) {
                        return string.Join(",", list);
                    } else
                        return value.ToString();
                }).ToArray()!;
            }
            if (vendor.FilterableAttributes != null) {
                Filterablekeys = vendor.FilterableAttributes.Keys.ToArray();
                FilterableDataValues = vendor.FilterableAttributes.Values.Select(value => {
                    return value.ToString();
                }).ToArray()!;
            }

            using (var command = new NpgsqlCommand($"SET search_path ={dbSchema}; " +
                $"CALL \"AddVendor\" (@name,@description,@link_to,@other_name,@public_name,@keys,@data_values,@data_type,@fkeys,@fdata_values,@logo)", connection)) {
                command.Parameters.AddWithValue("@name", vendor.Name!);
                command.Parameters.AddWithValue("@description",
                    vendor.Description! != null ? (object)vendor.Description : DBNull.Value);
                command.Parameters.AddWithValue("@link_to",
                    vendor.LinkTo! != null ? (object)vendor.LinkTo : DBNull.Value);
                command.Parameters.AddWithValue("@other_name",
                    vendor.OtherName! != null ? (object)vendor.OtherName : DBNull.Value);
                command.Parameters.AddWithValue("@public_name",
                    vendor.PublicName! != null ? (object)vendor.PublicName : DBNull.Value);
                command.Parameters.AddWithValue("@keys", keys != null ? keys : DBNull.Value);
                command.Parameters.AddWithValue("@data_values", dataValues != null ? dataValues : DBNull.Value);
                command.Parameters.AddWithValue("@data_type", dataType != null ? dataType : DBNull.Value);
                command.Parameters.AddWithValue("@fkeys", Filterablekeys != null ? Filterablekeys : DBNull.Value);
                command.Parameters.AddWithValue("@fdata_values", FilterableDataValues != null ? FilterableDataValues : DBNull.Value);
                command.Parameters.AddWithValue("@logo", vendor.Logo! != null ? (object)vendor.Logo : DBNull.Value);
                await command.ExecuteNonQueryAsync();
            }
            await connection.CloseAsync();
        } catch (Exception ex) {
            Console.WriteLine($"Detailed description of the error: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Обновить данные вендора в базе данных
    /// </summary>
    /// <param name="vendor">Объект вендора</param>
    public async Task UpdateDataVendorAsync(Vendor vendor) {
        try {
            await connection.OpenAsync();
            string[] keys = null;
            string[] dataValues = null;
            string[] Filterablekeys = null;
            string[] FilterableDataValues = null;
            List<string> dataTypeList = new List<string>();
            string[] dataType = null;
            Type valueType = null;
            string typeName = null;

            if (vendor.Attributes != null) {
                foreach (var kvp in vendor.Attributes) {
                    if (kvp.Value != null) {
                        if (kvp.Value is List<object>) {
                            valueType = kvp.Value.GetType();
                            if (valueType.IsGenericType) {
                                Type genericType = valueType.GetGenericArguments()[0];
                                typeName = genericType.Name;
                            }
                        } else {
                            valueType = kvp.Value.GetType();
                            typeName = valueType.Name;
                        }
                    }
                    dataTypeList.Add(typeName);
                }
                dataType = dataTypeList.ToArray();
            }
            if (vendor.Attributes != null) {
                keys = vendor.Attributes.Keys.ToArray();
                dataValues = vendor.Attributes.Values.Select(value => {
                    if (value is List<object> list) {
                        return string.Join(",", list);
                    } else if (value != null) {
                        return value.ToString();
                    } else {
                        return "null";
                    }
                }).ToArray()!;
            }
            if (vendor.FilterableAttributes != null) {
                Filterablekeys = vendor.FilterableAttributes.Keys.ToArray();
                FilterableDataValues = vendor.FilterableAttributes.Values.Select(value => {
                    return value.ToString();
                }).ToArray()!;
            }

            using (var command = new NpgsqlCommand($"SET search_path = {dbSchema}; CALL \"UpdateVendor\" " +
                $"(@flag_logo,@flag_link_to,@flag_description,@flag_other_name,@flag_public_name,@id,@name,@description," +
                $"@link_to,@other_name,@public_name,@keys,@data_values,@data_type,@fkeys,@fdata_values,@logo)", connection)) {
                command.Parameters.AddWithValue("@id", vendor.Id);
                command.Parameters.AddWithValue("@name", vendor.Name != null ? (object)vendor.Name : DBNull.Value);
                command.Parameters.AddWithValue("@description",
                    vendor.Description != null ? (object)vendor.Description : DBNull.Value);
                command.Parameters.AddWithValue("@link_to",
                    vendor.LinkTo != null ? (object)vendor.LinkTo : DBNull.Value);
                command.Parameters.AddWithValue("@other_name",
                    vendor.OtherName != null ? (object)vendor.OtherName : DBNull.Value);
                command.Parameters.AddWithValue("@public_name",
                    vendor.PublicName != null ? (object)vendor.PublicName : DBNull.Value);
                command.Parameters.AddWithValue("@flag_description", vendor.flagDescription);
                command.Parameters.AddWithValue("@flag_link_to", vendor.flagLinkTo);
                command.Parameters.AddWithValue("@flag_other_name", vendor.flagOtherName);
                command.Parameters.AddWithValue("@flag_public_name", vendor.flagPublicName);
                // параметры для таблиц guide_attribute и value_attribute
                command.Parameters.AddWithValue("@keys", keys != null && keys.Length > 0 ? keys : DBNull.Value);
                command.Parameters.AddWithValue("@data_values", dataValues != null && dataValues.Length > 0 ? dataValues : DBNull.Value);
                command.Parameters.AddWithValue("@data_type", dataType != null ? dataType : DBNull.Value);
                command.Parameters.AddWithValue("@fkeys", Filterablekeys != null ? Filterablekeys : DBNull.Value);
                command.Parameters.AddWithValue("@fdata_values", FilterableDataValues != null ? FilterableDataValues : DBNull.Value);
                command.Parameters.AddWithValue("@flag_logo", vendor.flagLogo);
                command.Parameters.AddWithValue("@logo", vendor.Logo! != null ? (object)vendor.Logo : DBNull.Value);
                await command.ExecuteNonQueryAsync();
            }
            await connection.CloseAsync();
        } catch (Exception ex) {
            Console.WriteLine($"Detailed description of the error: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Удалить вендора в базе данных по его id
    /// </summary>
    /// <param name="id">Id вендора</param>
    public async Task DeleteVendorByIdAsync(long id) {
        try {
            await connection.OpenAsync();
            using (NpgsqlCommand command = new NpgsqlCommand($"SET search_path = {dbSchema}; CALL \"DeleteVendorById\" (@id)", connection)) {
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
