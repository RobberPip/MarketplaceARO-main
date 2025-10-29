using RegistryApp.Domain.Enums;

namespace RegistryApp.Domain.Models {
    public class Filters {
        public string VendorName = "VendorName";
        public string SoftwareName = "SoftwareName";
        public string SoftwareDescription = "SoftwareDescription";
        public string SoftwareLinkTo = "SoftwareLinkTo";
        public string SoftwareLogo = "SoftwareLogo";
        public string search = "search";
        private List<string> filters;
        Dictionary<string, List<string>> uniqueProperties;
        /// <summary>
        /// Примеры использования метода:
        /// <br/>Поиск  - List&lt;string&gt;  filterList = software.filter.GetFilters(...,...);
        /// <br/>Поиск  - List&lt;string&gt;  filterList = software.filter.GetFilters(("Название атрибута",new List&lt;string/bool&gt;  {"Значени1,..."}),...);
        /// <br/>Поиск по другим полям - List&lt;string&gt; filterList = software.filter.GetFilters((new Filters().SoftwareDescription,Значение)...);
        /// <br/> Может принимать массив кортежей, а также обычные массивы значений
        /// </summary>
        /// <param name="filterParams"></param>
        /// <returns></returns>
        public List<string> GetFilters(params (string PropertyName, object? Value)[] filterParams) {
            filters = new List<string>();
            uniqueProperties = new Dictionary<string, List<string>>();

            var propertyToCondition = new Dictionary<string, string>
    {
        { "VendorName", "vendors.name" },
        { "SoftwareName", "software.name" },
        { "SoftwareDescription", "software.description" },
        { "SoftwareLinkTo", "software.link_to" },
        { "SoftwareLogo", "software.logo" },
        { "search", "val" }
    };

            foreach (var (propertyName, value) in filterParams) {
                if (!uniqueProperties.ContainsKey(propertyName))
                    uniqueProperties[propertyName] = new List<string>();

                GenerateCondition(propertyName, value, propertyToCondition);

            }

            foreach (var kvp in uniqueProperties) {
                if (kvp.Value.Count > 1)
                    filters.Add($"({string.Join(" OR ", kvp.Value)})");
                else
                    filters.AddRange(kvp.Value);
            }

            return filters;
        }

        private void GenerateCondition(string propertyName, object? value, Dictionary<string, string> propertyToCondition) {
            string[] formattedValues;

            switch (value) {
                case null:
                    formattedValues = CreateCondition(propertyName, value, propertyToCondition)
                    .Select(c => c.ToString())
                    .ToArray();
                    break;
                case string strValue:
                    formattedValues = strValue.Split(';').Select(word => CreateCondition(propertyName, word.Trim(), propertyToCondition)).ToArray();
                    break;
                case bool boolValue:
                    formattedValues = new[] { CreateCondition(propertyName, boolValue.ToString().ToLower(), propertyToCondition) };
                    break;
                case List<string> stringListValue:
                    formattedValues = stringListValue.Select(item => CreateCondition(propertyName, item.Trim(), propertyToCondition)).ToArray();
                    break;
                case List<int> intListValue:
                    formattedValues = intListValue.Select(intValue => CreateCondition(propertyName, intValue.ToString(), propertyToCondition)).ToArray();
                    break;
                default:
                    formattedValues = Array.Empty<string>();
                    break;
                    //TODO добавить другие типы данных
            }
            AddFilterToList(propertyName, formattedValues, filters, uniqueProperties);
        }
        private string CreateCondition(string propertyName, object? value, Dictionary<string, string> propertyToCondition) {
            if (value != null) {
                if (propertyName != "VendorName") {
                    if (!propertyToCondition.TryGetValue(propertyName, out string condition)) {
                        return $"software.id IN (SELECT software.id FROM software " +
                               $"LEFT JOIN value_attribute ON software.id = value_attribute.software_id " +
                               $"LEFT JOIN guide_attribute ON value_attribute.attribute_id = guide_attribute.id " +
                               $"WHERE guide_attribute.attribute_name = '{propertyName}' AND EXISTS (SELECT 1 FROM unnest(value_attribute.value) AS v " +
                               $"WHERE v ILIKE '%{value}%'))";
                    }

                    return $"{condition} ILIKE '%{value}%'";
                } else {
                    return $"software.id IN (SELECT software.id FROM software software " +
                                   $"LEFT JOIN software_vendors_links ON software.id = software_id " +
                                   "LEFT JOIN vendors ON software_vendors_links.vendor_id = vendors.id " +
                                   $"WHERE {propertyToCondition[propertyName]} ILIKE '%{value}%')";

                }
            } else {
                if (propertyToCondition.ContainsKey(propertyName)) {
                    return propertyToCondition[propertyName] + " IS NULL";
                } else {
                    return $"guide_attribute.attribute_name = '{propertyName}' AND value_attribute.value IS NULL";
                }
            }
        }

        private void AddFilterToList(string propertyName, string[] condition, List<string> filters, Dictionary<string, List<string>> uniqueProperties) {
            foreach (var item in condition) {
                // Проверка: должно ли поле внутри блока фильтра использовать логическое "и"
                if (!Enum.GetNames(typeof(FieldExceptions)).Contains(propertyName))
                    uniqueProperties[propertyName].Add(item);
                else
                    filters.Add(item);
            }
        }


    }
}