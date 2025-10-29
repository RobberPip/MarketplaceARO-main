using RegistryApp.Domain.Enums;
using RegistryApp.Domain.Models;
using Npgsql;

namespace RegistryApp.DB;

public partial class PostgreSqlAdapter : IDisposable {

    /// <summary>
    /// Получить все продукты
    /// </summary>
    public async Task<List<Software>> GetAllSoftwareAsync() {
        try {
            List<Software> softwareList = new List<Software>();
            await connection.OpenAsync();
            string query = @$"SET search_path = {dbSchema};
                            SELECT  software.id, software.name, software.description, software.link_to, software.logo, software.rating, software.review_count, software.like_count FROM software";

            List<long> masid = new List<long>();
            Software software = new Software();
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection)) {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        long Id = reader.GetInt64(0);
                        string? Name = reader.GetString(1);
                        string? Description = reader.IsDBNull(2) ? null : reader.GetString(2);
                        string? LinkTo = reader.IsDBNull(3) ? null : reader.GetString(3);
                        byte[]? Logo = reader.IsDBNull(4) ? null : (byte[])reader[4];
                        int rating = reader.GetInt32(5);
                        int reviewCount = reader.GetInt32(6);
                        int likeCount = reader.GetInt32(7);
                        masid.Add(Id);
                        software = softwareList.FirstOrDefault(s => s.Id == Id);

                        if (software == null) {
                            software = new Software(Id, Name, Description, LinkTo, Logo, new Dictionary<string, object>());
                            software.Rating = rating;
                            software.ReviewCount = reviewCount;
                            software.LikeCount = likeCount;
                            softwareList.Add(software);
                        }
                    }
                }
            }

            query = $@"SET search_path = {dbSchema}; SELECT attribute_name,value_attribute.value, software_id FROM value_attribute
                    JOIN software ON software_id = software.id
                    JOIN guide_attribute ON attribute_id = guide_attribute.id WHERE software_id IN (SELECT unnest(@masid));";

            using (var command = new NpgsqlCommand(query, connection)) {
                command.Parameters.AddWithValue("masid", masid);
                using (var reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        long softwareId = reader.GetInt64(2);
                        string NameAttribute = reader.GetString(0);
                        object[] ValueAttribute = (object[])reader.GetValue(1);
                        software = softwareList.FirstOrDefault(s => s.Id == softwareId);
                        if (software != null) {
                            if (!software.Attributes.ContainsKey(NameAttribute)) {
                                software.Attributes[NameAttribute] = ValueAttribute;
                            }
                        }
                    }
                }
            }

            query = $@"SET search_path = {dbSchema}; SELECT id, name, description, link_to, other_name, svl.software_id, logo FROM vendors
                                JOIN software_vendors_links AS svl ON vendors.id = svl.vendor_id
                                WHERE svl.software_id IN (SELECT unnest(@masid));";

            using (var command = new NpgsqlCommand(query, connection)) {
                command.Parameters.AddWithValue("masid", masid);
                using (var reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        long softwareId = reader.GetInt64(5);
                        long idVendor = reader.GetInt64(0);
                        string? NameVendor = reader.GetString(1);
                        string? DescriptionVendor = reader.IsDBNull(2) ? null : reader.GetString(2);
                        string? LinkToVendor = reader.IsDBNull(3) ? null : reader.GetString(3);
                        string? otherNameVendor = reader.IsDBNull(4) ? null : reader.GetString(4);
                        byte[]? Logo = reader.IsDBNull(6) ? null : (byte[])reader[6];
                        software = softwareList.FirstOrDefault(s => s.Id == softwareId);
                        if (software != null) {
                            Vendor vendor = new Vendor() {
                                Id = idVendor,
                                Name = NameVendor,
                                Description = DescriptionVendor,
                                LinkTo = LinkToVendor,
                                OtherName = otherNameVendor,
                                Logo = Logo
                            };
                            software.Vendors.Add(vendor);
                        }
                    }
                }
            }
            await connection.CloseAsync();
            return softwareList;
        } catch (Exception ex) {
            Console.WriteLine($"Detailed description of the error: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Получить запрашиваемое количество продуктов в отсортированном виде
    /// </summary>
    /// <param name="sortBy">Тип сортировки</param>
    /// <param name="limit">Количество запрашиваемых продуктов (по умолчанию - все)</param>
    public async Task<List<Software>> GetAllSoftwareSortedAsync(SortBy sortBy, int? limit = null, string? userId = null) {
        try {
            List<Software> softwareList = new List<Software>();
            string orderByColumn = "";
            string Limit = "";

            if (sortBy == SortBy.Name)
                orderByColumn = "ORDER BY software.name";
            else if (sortBy == SortBy.Date)
                orderByColumn = "ORDER BY software.date_to_add DESC";
            else if (sortBy == SortBy.LikeCount)
                orderByColumn = "ORDER BY software.like_count DESC";
            else
                orderByColumn = null;

            if (limit.HasValue)
                Limit += $" LIMIT {limit.Value}";

            await connection.OpenAsync();
            string query = @$"SET search_path = {dbSchema}; 
                  SELECT software.id, software.name, software.description, 
                         software.link_to, software.logo, software.rating, 
                         software.review_count, software.like_count " + (userId != null ? $@", 
                CASE 
                             WHEN ""Reviews"".""Like"" = TRUE THEN TRUE 
                             ELSE FALSE 
                         END AS ""LikedByUser""" : "") +
                  "FROM software ";

            if (userId != null) {
                query += @$" LEFT JOIN ""Reviews"" ON software.id = ""LegacySoftwareId"" 
                 AND ""AuthorId"" = '{userId}'";
            }

            query += $" {orderByColumn} {Limit}";


            List<long> masid = new List<long>();
            Software software = new Software();
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection)) {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        long Id = reader.GetInt64(0);
                        string? Name = reader.GetString(1);
                        string? Description = reader.IsDBNull(2) ? null : reader.GetString(2);
                        string? LinkTo = reader.IsDBNull(3) ? null : reader.GetString(3);
                        byte[]? Logo = reader.IsDBNull(4) ? null : (byte[])reader[4];
                        int rating = reader.GetInt32(5);
                        int reviewCount = reader.GetInt32(6);
                        int likeCount = reader.GetInt32(7);
                        bool isLiked = reader.FieldCount > 8 && !reader.IsDBNull(8) ? reader.GetBoolean(8) : false;
                        masid.Add(Id);
                        software = softwareList.FirstOrDefault(s => s.Id == Id);

                        if (software == null) {
                            software = new Software(Id, Name, Description, LinkTo, Logo, new Dictionary<string, object>());
                            software.Rating = rating;
                            software.ReviewCount = reviewCount;
                            software.LikeCount = likeCount;
                            software.IsLiked = isLiked;
                            softwareList.Add(software);
                        }
                    }
                }
            }

            query = $@"SET search_path = {dbSchema}; SELECT attribute_name,value_attribute.value, software_id FROM value_attribute
                    JOIN software ON software_id = software.id
                    JOIN guide_attribute ON attribute_id = guide_attribute.id WHERE software_id IN (SELECT unnest(@masid));";

            using (var command = new NpgsqlCommand(query, connection)) {
                command.Parameters.AddWithValue("masid", masid);
                using (var reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        long softwareId = reader.GetInt64(2);
                        string NameAttribute = reader.IsDBNull(0) ? null : reader.GetString(0);
                        object[] ValueAttribute = reader.IsDBNull(1) ? null : (object[])reader.GetValue(1);
                        software = softwareList.FirstOrDefault(s => s.Id == softwareId);
                        if (software != null) {
                            if (!software.Attributes.ContainsKey(NameAttribute)) {
                                software.Attributes[NameAttribute] = ValueAttribute;
                            }
                        }
                    }
                }
            }

            query = $@"SET search_path = {dbSchema}; SELECT id, name, description, link_to, other_name, svl.software_id, logo FROM vendors
                                JOIN software_vendors_links AS svl ON vendors.id = svl.vendor_id
                                WHERE svl.software_id IN (SELECT unnest(@masid));";

            using (var command = new NpgsqlCommand(query, connection)) {
                command.Parameters.AddWithValue("masid", masid);
                using (var reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        long softwareId = reader.GetInt64(5);
                        long idVendor = reader.GetInt64(0);
                        string? NameVendor = reader.GetString(1);
                        string? DescriptionVendor = reader.IsDBNull(2) ? null : reader.GetString(2);
                        string? LinkToVendor = reader.IsDBNull(3) ? null : reader.GetString(3);
                        string? otherNameVendor = reader.IsDBNull(4) ? null : reader.GetString(4);
                        byte[]? Logo = reader.IsDBNull(6) ? null : (byte[])reader[6];
                        software = softwareList.FirstOrDefault(s => s.Id == softwareId);
                        if (software != null) {
                            Vendor vendor = new Vendor() {
                                Id = idVendor,
                                Name = NameVendor,
                                Description = DescriptionVendor,
                                LinkTo = LinkToVendor,
                                OtherName = otherNameVendor,
                                Logo = Logo
                            };
                            software.Vendors.Add(vendor);
                        }
                    }
                }
            }
            await connection.CloseAsync();
            return softwareList;
        } catch (Exception ex) {
            Console.WriteLine($"Detailed description of the error: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Получить продукты с учетом фильтрации
    /// </summary>
    /// <param name="filterList">Список фильтров</param>
    public async Task<List<Software>> GetSoftwareFilterAsync(List<string> filterList) {
        try {
            List<Software> softwareList = new List<Software>();
            await connection.OpenAsync();
            string query = @$"SET search_path = {dbSchema}; WITH q1 AS (SELECT id as sid, name as val, description, link_to, logo, date_to_add, 0 AS src FROM software),
        q2 AS (SELECT attribute_id as aid, unnest(value) as val, type_data AS type, software_id as sid, vendor_id as vid, 1 AS src FROM value_attribute),
        q3 AS (SELECT id, vendors.name as val, 2 AS src FROM vendors),
        combined_result AS (SELECT  sid, val, description, link_to, date_to_add, src, aid, vid, type
         FROM (SELECT q1.sid, q1.val, q1.description, q1.link_to, q1.date_to_add, q1.src, null as aid, null as vid, null as type
               FROM q1
               union all
               SELECT q2.sid, q2.val, null as description, null as link_to, null as date_to_add, q2.src, q2.aid, q2.vid, q2.type
               FROM q2
               UNION ALL
               SELECT svl.software_id AS sid,q3.val,null ,null ,null ,q3.src,null, null, null
               FROM q3 JOIN software_vendors_links AS svl ON vendor_id = q3.id) AS combined_result_temp)

        SELECT DISTINCT ON (software.id, software.name, software.date_to_add) software.id, software.name, software.description, software.link_to, software.logo, software.rating, software.review_count, software.like_count, combined_result.src
        FROM software
        JOIN combined_result ON software.id = combined_result.sid
        LEFT JOIN value_attribute ON software.id = value_attribute.software_id
        LEFT JOIN guide_attribute ON guide_attribute.id = value_attribute.attribute_id
        LEFT JOIN software_vendors_links ON software.id = software_vendors_links.software_id
        LEFT JOIN vendors ON software_vendors_links.vendor_id = vendors.id
        WHERE {string.Join(" AND ", filterList)} AND (NOT EXISTS (SELECT 1 FROM guide_attribute WHERE software.id = value_attribute.software_id) OR
        guide_attribute.filterable_attribute = true)";

            List<long> masid = new List<long>();
            Software software = new Software();
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection)) {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        long Id = reader.GetInt64(0);
                        string? Name = reader.GetString(1);
                        string? Description = reader.IsDBNull(2) ? null : reader.GetString(2);
                        string? LinkTo = reader.IsDBNull(3) ? null : reader.GetString(3);
                        byte[]? Logo = reader.IsDBNull(4) ? null : (byte[])reader[4];
                        int rating = reader.GetInt32(5);
                        int reviewCount = reader.GetInt32(6);
                        int likeCount = reader.GetInt32(7);
                        masid.Add(Id);
                        software = softwareList.FirstOrDefault(s => s.Id == Id);

                        if (software == null) {
                            software = new Software(Id, Name, Description, LinkTo, Logo, new Dictionary<string, object>());
                            software.Rating = rating;
                            software.ReviewCount = reviewCount;
                            software.LikeCount = likeCount;
                            softwareList.Add(software);
                        }
                    }
                }
            }

            query = $@"SET search_path = {dbSchema}; SELECT attribute_name,value_attribute.value, software_id FROM value_attribute
                    JOIN software ON software_id = software.id
                    JOIN guide_attribute ON attribute_id = guide_attribute.id WHERE software_id IN (SELECT unnest(@masid));";

            using (var command = new NpgsqlCommand(query, connection)) {
                command.Parameters.AddWithValue("masid", masid);
                using (var reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        long softwareId = reader.GetInt64(2);
                        string NameAttribute = reader.GetString(0);
                        object[] ValueAttribute = (object[])reader.GetValue(1);
                        software = softwareList.FirstOrDefault(s => s.Id == softwareId);
                        if (software != null) {
                            if (!software.Attributes.ContainsKey(NameAttribute)) {
                                software.Attributes[NameAttribute] = ValueAttribute;
                            }
                        }
                    }
                }
            }

            query = $@"SET search_path = {dbSchema}; SELECT id, name, description, link_to, other_name, svl.software_id, logo FROM vendors
                                JOIN software_vendors_links AS svl ON vendors.id = svl.vendor_id
                                WHERE svl.software_id IN (SELECT unnest(@masid));";

            using (var command = new NpgsqlCommand(query, connection)) {
                command.Parameters.AddWithValue("masid", masid);
                using (var reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        long softwareId = reader.GetInt64(5);
                        long idVendor = reader.GetInt64(0);
                        string? NameVendor = reader.GetString(1);
                        string? DescriptionVendor = reader.IsDBNull(2) ? null : reader.GetString(2);
                        string? LinkToVendor = reader.IsDBNull(3) ? null : reader.GetString(3);
                        string? otherNameVendor = reader.IsDBNull(4) ? null : reader.GetString(4);
                        byte[]? Logo = reader.IsDBNull(6) ? null : (byte[])reader[6];
                        software = softwareList.FirstOrDefault(s => s.Id == softwareId);
                        if (software != null) {
                            Vendor vendor = new Vendor() {
                                Id = idVendor,
                                Name = NameVendor,
                                Description = DescriptionVendor,
                                LinkTo = LinkToVendor,
                                OtherName = otherNameVendor,
                                Logo = Logo
                            };
                            software.Vendors.Add(vendor);
                        }
                    }
                }
            }
            await connection.CloseAsync();
            return softwareList;
        } catch (Exception ex) {
            Console.WriteLine($"Detailed description of the error: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Получить продукты в отсортированном виде для постраничной выдачи
    /// </summary>
    /// <param name="sortBy">Тип сортировки</param>
    /// <param name="pageNumber">Номер страницы для выдачи</param>
    /// <param name="itemsPerPage">Количество продуктов на страницу</param>
    public async Task<List<Software>> GetAllSoftwareSortedAsync(SortBy sortBy, int pageNumber, int itemsPerPage, string? userId = null) {
        try {
            var offset = (pageNumber - 1) * itemsPerPage; // выражение для подсчёта, с какой строки считывать данные
            if (pageNumber == 1)
                offset = 0; // нужно чтобы первая страница начиналась с 0

            List<Software> softwareList = new List<Software>();
            string orderByColumn = "";

            if (sortBy == SortBy.Name)
                orderByColumn = "ORDER BY software.name, software.id";
            else if (sortBy == SortBy.Date)
                orderByColumn = "ORDER BY software.date_to_add DESC, software.id";
            else if (sortBy == SortBy.LikeCount)
                orderByColumn = "ORDER BY software.like_count DESC, software.id";
            else
                orderByColumn = null;

            await connection.OpenAsync();


            string query = @$"
                SET search_path = {dbSchema};

                SELECT software.id, software.name, software.description, software.link_to, 
                    software.logo, software.rating, software.review_count, software.like_count" +
                    (userId != null ? @",
                    EXISTS (
                        SELECT 1 FROM ""Reviews""
                        WHERE ""LegacySoftwareId"" = software.id 
                            AND ""AuthorId"" = '" + userId + @"'
                            AND ""Like"" = TRUE
                    ) AS ""LikedByUser""" : "") + @"
                FROM software
                " + orderByColumn + $@"
                OFFSET {offset} LIMIT {itemsPerPage};";


            List<long> masid = new List<long>();
            Software software = new Software();
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection)) {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        long Id = reader.GetInt64(0);
                        string? Name = reader.GetString(1);
                        string? Description = reader.IsDBNull(2) ? null : reader.GetString(2);
                        string? LinkTo = reader.IsDBNull(3) ? null : reader.GetString(3);
                        byte[]? Logo = reader.IsDBNull(4) ? null : (byte[])reader[4];
                        int rating = reader.GetInt32(5);
                        int reviewCount = reader.GetInt32(6);
                        int likeCount = reader.GetInt32(7);
                        bool isLiked = reader.FieldCount > 8 && !reader.IsDBNull(8) ? reader.GetBoolean(8) : false;
                        masid.Add(Id);
                        software = softwareList.FirstOrDefault(s => s.Id == Id);

                        if (software == null) {
                            software = new Software(Id, Name, Description, LinkTo, Logo, new Dictionary<string, object>());
                            software.Rating = rating;
                            software.ReviewCount = reviewCount;
                            software.LikeCount = likeCount;
                            software.IsLiked = isLiked;
                            softwareList.Add(software);
                        }
                    }
                }
            }

            query = $@"SET search_path = {dbSchema}; SELECT attribute_name,value_attribute.value, software_id FROM value_attribute
                JOIN software ON software_id = software.id
                JOIN guide_attribute ON attribute_id = guide_attribute.id WHERE software_id IN (SELECT unnest(@masid));";

            using (var command = new NpgsqlCommand(query, connection)) {
                command.Parameters.AddWithValue("masid", masid);
                using (var reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        long softwareId = reader.GetInt64(2);
                        string NameAttribute = reader.GetString(0);
                        object[] ValueAttribute = (object[])reader.GetValue(1);
                        software = softwareList.FirstOrDefault(s => s.Id == softwareId);
                        if (software != null) {
                            if (!software.Attributes.ContainsKey(NameAttribute)) {
                                software.Attributes[NameAttribute] = ValueAttribute;
                            }
                        }
                    }
                }
            }

            query = $@"SET search_path = {dbSchema}; SELECT id, name, description, link_to, other_name, svl.software_id, logo FROM vendors
                                JOIN software_vendors_links AS svl ON vendors.id = svl.vendor_id
                                WHERE svl.software_id IN (SELECT unnest(@masid));";

            using (var command = new NpgsqlCommand(query, connection)) {
                command.Parameters.AddWithValue("masid", masid);
                using (var reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        long softwareId = reader.GetInt64(5);
                        long idVendor = reader.GetInt64(0);
                        string? NameVendor = reader.GetString(1);
                        string? DescriptionVendor = reader.IsDBNull(2) ? null : reader.GetString(2);
                        string? LinkToVendor = reader.IsDBNull(3) ? null : reader.GetString(3);
                        string? otherNameVendor = reader.IsDBNull(4) ? null : reader.GetString(4);
                        byte[]? Logo = reader.IsDBNull(6) ? null : (byte[])reader[6];
                        software = softwareList.FirstOrDefault(s => s.Id == softwareId);
                        if (software != null) {
                            Vendor vendor = new Vendor() {
                                Id = idVendor,
                                Name = NameVendor,
                                Description = DescriptionVendor,
                                LinkTo = LinkToVendor,
                                OtherName = otherNameVendor,
                                Logo = Logo
                            };
                            software.Vendors.Add(vendor);
                        }
                    }
                }
            }
            await connection.CloseAsync();
            return softwareList;
        } catch (Exception ex) {
            Console.WriteLine($"Detailed description of the error: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Получить продукты в отсортированном виде с учетом фильтрации для постраничной выдачи
    /// </summary>
    /// <param name="filterList">Список фильтров</param
    /// <param name="sortBy">Тип сортировки</param>
    /// <param name="pageNumber">Номер страницы для выдачи</param>
    /// <param name="itemsPerPage">Количество продуктов на страницу</param>
    public async Task<List<Software>> GetSoftwareFilterSortedAsync(List<string> filterList, SortBy sortBy, int pageNumber, int itemsPerPage, string? userId = null) {
        try {
            int offset = (pageNumber - 1) * itemsPerPage;
            if (pageNumber == 1)
                offset = 0;

            List<Software> softwareList = new List<Software>();
            string orderByColumn = "";

            if (sortBy == SortBy.Name)
                orderByColumn = "ORDER BY software.name";
            else if (sortBy == SortBy.Date)
                orderByColumn = "ORDER BY software.date_to_add DESC";
            else if (sortBy == SortBy.LikeCount)
                orderByColumn = "ORDER BY software.like_count DESC";
            else
                orderByColumn = null;

            await connection.OpenAsync();
            string query =
                    @$"SET search_path = {dbSchema}; WITH q1 AS (SELECT id as sid, name as val, description, link_to, logo, date_to_add, 0 AS src FROM software),
        q2 AS (SELECT attribute_id as aid, unnest(value) as val, type_data AS type, software_id as sid, vendor_id as vid, 1 AS src FROM value_attribute),
        q3 AS (SELECT id, vendors.name as val, 2 AS src FROM vendors),
        combined_result AS (SELECT  sid, val, description, link_to, date_to_add, src, aid, vid, type
         FROM (SELECT q1.sid, q1.val, q1.description, q1.link_to, q1.date_to_add, q1.src, null as aid, null as vid, null as type
               FROM q1
               union all
               SELECT q2.sid, q2.val, null as description, null as link_to, null as date_to_add, q2.src, q2.aid, q2.vid, q2.type
               FROM q2
               UNION ALL
               SELECT svl.software_id AS sid,q3.val,null ,null ,null ,q3.src,null, null, null
               FROM q3 JOIN software_vendors_links AS svl ON vendor_id = q3.id) AS combined_result_temp)

        SELECT DISTINCT ON (software.id, software.name, software.date_to_add, software.like_count) software.id, software.name, software.description, software.link_to, software.logo, 
        software.rating, software.review_count, software.like_count " + (userId != null ? $@", 
            CASE 
                WHEN ""Reviews"".""Like"" = TRUE THEN TRUE 
            ELSE FALSE 
            END AS ""LikedByUser""" : "") +
                    $@" FROM software
        JOIN combined_result ON software.id = combined_result.sid
        LEFT JOIN value_attribute ON software.id = value_attribute.software_id
        LEFT JOIN guide_attribute ON guide_attribute.id = value_attribute.attribute_id
        LEFT JOIN software_vendors_links ON software.id = software_vendors_links.software_id
        LEFT JOIN vendors ON software_vendors_links.vendor_id = vendors.id " +
                    (userId != null ?
                        @$"LEFT JOIN ""Reviews"" ON software.id = ""LegacySoftwareId"" AND ""AuthorId"" = '{userId}' " : "") +
                    $@" WHERE {string.Join(" AND ", filterList)} AND (NOT EXISTS (SELECT 1 FROM guide_attribute WHERE software.id = value_attribute.software_id) OR
        guide_attribute.filterable_attribute = true) " +
    $"{orderByColumn} " +
    $"OFFSET {offset} " +
    $"LIMIT {itemsPerPage}";

            List<long> masid = new List<long>();
            Software software = new Software();
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection)) {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        long Id = reader.GetInt64(0);
                        string? Name = reader.GetString(1);
                        string? Description = reader.IsDBNull(2) ? null : reader.GetString(2);
                        string? LinkTo = reader.IsDBNull(3) ? null : reader.GetString(3);
                        byte[]? Logo = reader.IsDBNull(4) ? null : (byte[])reader[4];
                        int rating = reader.GetInt32(5);
                        int reviewCount = reader.GetInt32(6);
                        int likeCount = reader.GetInt32(7);
                        bool isLiked = reader.FieldCount > 8 && !reader.IsDBNull(8) ? reader.GetBoolean(8) : false;
                        masid.Add(Id);
                        software = softwareList.FirstOrDefault(s => s.Id == Id);

                        if (software == null) {
                            software = new Software(Id, Name, Description, LinkTo, Logo, new Dictionary<string, object>());
                            software.Rating = rating;
                            software.ReviewCount = reviewCount;
                            software.LikeCount = likeCount;
                            software.IsLiked = isLiked;
                            softwareList.Add(software);
                        }
                    }
                }
            }

            query = $@"SET search_path = {dbSchema}; SELECT attribute_name,value_attribute.value, software_id FROM value_attribute
                JOIN software ON software_id = software.id
                JOIN guide_attribute ON attribute_id = guide_attribute.id WHERE software_id IN (SELECT unnest(@masid));";

            using (var command = new NpgsqlCommand(query, connection)) {
                command.Parameters.AddWithValue("masid", masid);
                using (var reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        long softwareId = reader.GetInt64(2);
                        string NameAttribute = reader.GetString(0);
                        object[] ValueAttribute = (object[])reader.GetValue(1);
                        software = softwareList.FirstOrDefault(s => s.Id == softwareId);
                        if (software != null) {
                            if (!software.Attributes.ContainsKey(NameAttribute)) {
                                software.Attributes[NameAttribute] = ValueAttribute;
                            }
                        }
                    }
                }
            }

            query = $@"SET search_path = {dbSchema};  SELECT id, name, description, link_to, other_name, svl.software_id, logo FROM vendors
                                JOIN software_vendors_links AS svl ON vendors.id = svl.vendor_id
                                WHERE svl.software_id IN (SELECT unnest(@masid));";

            using (var command = new NpgsqlCommand(query, connection)) {
                command.Parameters.AddWithValue("masid", masid);
                using (var reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        long softwareId = reader.GetInt64(5);
                        long idVendor = reader.GetInt64(0);
                        string? NameVendor = reader.GetString(1);
                        string? DescriptionVendor = reader.IsDBNull(2) ? null : reader.GetString(2);
                        string? LinkToVendor = reader.IsDBNull(3) ? null : reader.GetString(3);
                        string? otherNameVendor = reader.IsDBNull(4) ? null : reader.GetString(4);
                        byte[]? Logo = reader.IsDBNull(6) ? null : (byte[])reader[6];
                        software = softwareList.FirstOrDefault(s => s.Id == softwareId);
                        if (software != null) {
                            Vendor vendor = new Vendor() {
                                Id = idVendor,
                                Name = NameVendor,
                                Description = DescriptionVendor,
                                LinkTo = LinkToVendor,
                                OtherName = otherNameVendor,
                                Logo = Logo
                            };
                            software.Vendors.Add(vendor);
                        }
                    }
                }
            }
            await connection.CloseAsync();
            return softwareList;
        } catch (Exception ex) {
            Console.WriteLine($"Detailed description of the error: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Получить продукт по id
    /// </summary>
    /// <param name="id">Id продукта</param
    public async Task<Software> GetSoftwareByIdAsync(long id, string? userId = null) {
        try {
            Software software = new Software();
            await connection.OpenAsync();
            string query = @$"SET search_path = {dbSchema}; 
                    SELECT software.id, software.name, software.description, software.link_to, software.logo, software.rating, software.review_count, software.like_count "
                           + (userId != null
                               ? $@", 
            CASE 
                WHEN ""Reviews"".""Like"" = TRUE THEN TRUE 
            ELSE FALSE 
            END AS ""LikedByUser"""
                               : "") + " FROM software ";
            if (userId != null) {
                query += @$" LEFT JOIN ""Reviews"" ON software.id = ""LegacySoftwareId"" 
                 AND ""AuthorId"" = '{userId}'";
            }
            query += " WHERE software.id = @Id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection)) {
                command.Parameters.AddWithValue("Id", id);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        long Id = reader.GetInt64(0);
                        string? Name = reader.GetString(1);
                        string? Description = reader.IsDBNull(2) ? null : reader.GetString(2);
                        string? LinkTo = reader.IsDBNull(3) ? null : reader.GetString(3);
                        byte[]? Logo = reader.IsDBNull(4) ? null : (byte[])reader[4];
                        int rating = reader.GetInt32(5);
                        int reviewCount = reader.GetInt32(6);
                        int likeCount = reader.GetInt32(7);
                        bool isLiked = reader.FieldCount > 8 && !reader.IsDBNull(8) ? reader.GetBoolean(8) : false;
                        software = new Software(Id, Name, Description, LinkTo, Logo, new Dictionary<string, object>());
                        software.Rating = rating;
                        software.ReviewCount = reviewCount;
                        software.LikeCount = likeCount;
                        software.IsLiked = isLiked;
                    }
                }
            }

            query = $@"SET search_path = {dbSchema}; SELECT attribute_name,value_attribute.value, software_id FROM value_attribute
                    JOIN software ON software_id = software.id
                    JOIN guide_attribute ON attribute_id = guide_attribute.id WHERE software_id = @Id;";

            using (var command = new NpgsqlCommand(query, connection)) {
                command.Parameters.AddWithValue("Id", id);
                using (var reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        long softwareId = reader.GetInt64(2);
                        string NameAttribute = reader.GetString(0);
                        object[] ValueAttribute = (object[])reader.GetValue(1);
                        if (software != null) {
                            if (!software.Attributes.ContainsKey(NameAttribute)) {
                                software.Attributes[NameAttribute] = ValueAttribute;
                            }
                        }
                    }
                }
            }

            query = $@"SET search_path = {dbSchema}; SELECT id, name, description, link_to, other_name, svl.software_id, logo FROM vendors
                                JOIN software_vendors_links AS svl ON vendors.id = svl.vendor_id
                                WHERE svl.software_id = @Id;";

            using (var command = new NpgsqlCommand(query, connection)) {
                command.Parameters.AddWithValue("Id", id);
                using (var reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        long softwareId = reader.GetInt64(5);
                        long idVendor = reader.GetInt64(0);
                        string? NameVendor = reader.GetString(1);
                        string? DescriptionVendor = reader.IsDBNull(2) ? null : reader.GetString(2);
                        string? LinkToVendor = reader.IsDBNull(3) ? null : reader.GetString(3);
                        string? otherNameVendor = reader.IsDBNull(4) ? null : reader.GetString(4);
                        byte[]? Logo = reader.IsDBNull(6) ? null : (byte[])reader[6];
                        if (software != null) {
                            Vendor vendor = new Vendor() {
                                Id = idVendor,
                                Name = NameVendor,
                                Description = DescriptionVendor,
                                LinkTo = LinkToVendor,
                                OtherName = otherNameVendor,
                                Logo = Logo
                            };
                            software.Vendors.Add(vendor);
                        }
                    }
                }
            }
            await connection.CloseAsync();
            return software;
        } catch (Exception ex) {
            Console.WriteLine($"Detailed description of the error: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Добавить продукт в базу данных
    /// Если при вставке имя вендора указано не верно/не указано то в БД будет null,
    /// При вставке атрибутов - названия атрибутов (ключей в коллекции) не должны повторяться
    /// </summary>
    /// <param name="software">Объект продукта</param>
    public async Task AddSoftwareAsync(Software software) {
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

            if (software.Attributes != null) {
                foreach (var kvp in software.Attributes) {
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
            if (software.Attributes != null) {
                keys = software.Attributes.Keys.ToArray();
                dataValues = software.Attributes.Values.Select(value => {
                    if (value is List<object> list) {
                        return string.Join(",,", list);
                    } else
                        return value.ToString();
                }).ToArray()!;
            }
            if (software.FilterableAttributes != null) {
                Filterablekeys = software.FilterableAttributes.Keys.ToArray();
                FilterableDataValues = software.FilterableAttributes.Values.Select(value => {
                    return value.ToString();
                }).ToArray()!;
            }

            using (NpgsqlCommand command = new NpgsqlCommand($"CALL {dbSchema}.\"AddSoftware\" (@name,@logo,@description,@link_to,@keys,@data_values,@vendor,@data_type,@fkeys,@fdata_values)", connection)) {
                command.Parameters.AddWithValue("@name", software.Name!);
                command.Parameters.AddWithValue("@description", software.Description! != null ? (object)software.Description : DBNull.Value);
                command.Parameters.AddWithValue("@logo", software.Logo! != null ? (object)software.Logo : DBNull.Value);
                command.Parameters.AddWithValue("@vendor", software.Vendor != null ? (object)software.Vendor : DBNull.Value);
                command.Parameters.AddWithValue("@link_to", software.LinkTo! != null ? (object)software.LinkTo : DBNull.Value);
                command.Parameters.AddWithValue("@keys", keys != null ? keys : DBNull.Value);
                command.Parameters.AddWithValue("@data_values", dataValues != null ? dataValues : DBNull.Value);
                command.Parameters.AddWithValue("@data_type", dataType != null ? dataType : DBNull.Value);
                command.Parameters.AddWithValue("@fkeys", Filterablekeys != null ? Filterablekeys : DBNull.Value);
                command.Parameters.AddWithValue("@fdata_values", FilterableDataValues != null ? FilterableDataValues : DBNull.Value);
                await command.ExecuteNonQueryAsync();
            }
            await connection.CloseAsync();
        } catch (Exception ex) {
            Console.WriteLine($"Detailed description of the error: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Обновить данные продукта
    /// Если при вставке имя вендора не указано/указано не верно то в БД связи с вендором не будет,
    /// При вставке атрибутов - названия атрибутов (ключей в коллекции) не должны повторяться,
    /// Нужно чтобы id software имело значение т.к. поиск идёт по id
    /// </summary>
    /// <param name="software">Объект продукта</param>
    public async Task UpdateDataSoftwareAsync(Software software) {
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

            if (software.Attributes != null) {
                foreach (var kvp in software.Attributes) {
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
            if (software.Attributes != null) {
                keys = software.Attributes.Keys.ToArray();
                dataValues = software.Attributes.Values.Select(value => {
                    if (value is List<object> list) {
                        return string.Join(",,", list);
                    } else if (value != null) {
                        return value.ToString();
                    } else {
                        return "null";
                    }
                }).ToArray()!;
            }
            if (software.FilterableAttributes != null) {
                Filterablekeys = software.FilterableAttributes.Keys.ToArray();
                FilterableDataValues = software.FilterableAttributes.Values.Select(value => {
                    return value.ToString();
                }).ToArray()!;
            }

            using (NpgsqlCommand command = new NpgsqlCommand($"CALL {dbSchema}.\"UpdateSoftware\" " +
                $"(@id,@flag_logo,@flag_vendor,@flag_link_to,@flag_description,@name,@description,@link_to,@logo,@vendor," +
                $"@keys,@data_values,@data_type,@fkeys,@fdata_values)", connection)) {
                command.Parameters.AddWithValue("@id", software.Id!);
                command.Parameters.AddWithValue("@name",
                    software.Name != null ? (object)software.Name : DBNull.Value);
                command.Parameters.AddWithValue("@description",
                    software.Description! != null ? (object)software.Description : DBNull.Value);
                command.Parameters.AddWithValue("@logo",
                    software.Logo! != null ? (object)software.Logo : DBNull.Value);
                command.Parameters.AddWithValue("@vendor",
                    software.Vendor != null ? (object)software.Vendor : DBNull.Value);
                command.Parameters.AddWithValue("@link_to",
                    software.LinkTo! != null ? (object)software.LinkTo : DBNull.Value);
                command.Parameters.AddWithValue("@flag_description", software.flagDescription);
                command.Parameters.AddWithValue("@flag_logo", software.flagLogo);
                command.Parameters.AddWithValue("@flag_link_to", software.flagLinkTo);
                command.Parameters.AddWithValue("@flag_vendor", software.flagVendor);
                // параметры для таблиц guide_attribute и value_attribute
                command.Parameters.AddWithValue("@keys", keys != null && keys.Length > 0 ? keys : DBNull.Value);
                command.Parameters.AddWithValue("@data_values", dataValues != null && dataValues.Length > 0 ? dataValues : DBNull.Value);
                command.Parameters.AddWithValue("@data_type", dataType != null ? dataType : DBNull.Value);
                command.Parameters.AddWithValue("@fkeys", Filterablekeys != null ? Filterablekeys : DBNull.Value);
                command.Parameters.AddWithValue("@fdata_values", FilterableDataValues != null ? FilterableDataValues : DBNull.Value);
                await command.ExecuteNonQueryAsync();
            }
            await connection.CloseAsync();
        } catch (Exception ex) {
            Console.WriteLine($"Detailed description of the error: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Удалить продукт по id
    /// </summary>
    /// <param name="id">Id продукта</param
    public async Task DeleteSoftwareByIdAsync(long id) {
        try {
            await connection.OpenAsync();
            using (NpgsqlCommand command = new NpgsqlCommand($"CALL {dbSchema}.\"DeleteSoftwareById\" (@id)", connection)) {
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
