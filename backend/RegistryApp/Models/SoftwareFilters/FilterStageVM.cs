using System.Text.Json.Serialization;

namespace RegistryApp.Models.SoftwareFilters {
    public class FilterStageVM {
        [JsonPropertyName("company_filter")]
        public List<string> Companies { get; set; }

        [JsonPropertyName("activity_filter")]
        public string Activity { get; set; }

        [JsonPropertyName("softwareType_filter")]
        public string SoftwareType { get; set; }

        [JsonPropertyName("mainproduct_filter")]
        public List<string> MainProducts { get; set; }

        [JsonPropertyName("stage_filter")]
        public List<string> Stages { get; set; }

        [JsonPropertyName("content_filter")]
        public List<string> Contents { get; set; }

        [JsonPropertyName("workwithCIM_filter")]
        public string WorkWithCim { get; set; }

        [JsonPropertyName("regReestr_filter")]
        public string RegReestr { get; set; }

        [JsonPropertyName("isNotimMember_filter")]
        public string IsNotimMember { get; set; }

        [JsonPropertyName("search_request")]
        public string SearchRequest { get; set; }

        public FilterStageVM() { }

        public FilterStageVM(Dictionary<string, string> filterData) {
            Companies = GetValueArray(filterData, "company_filter");
            Activity = GetValueString(filterData, "activity_filter");
            SoftwareType = GetValueString(filterData, "softwareType_filter");
            MainProducts = GetValueArray(filterData, "mainproduct_filter");
            Stages = GetValueArray(filterData, "stage_filter");
            Contents = GetValueArray(filterData, "content_filter");

            WorkWithCim = GetValueString(filterData, "workwithCIM_filter");
            RegReestr = GetValueString(filterData, "regReestr_filter");
            IsNotimMember = GetValueString(filterData, "isNotimMember_filter");
            SearchRequest = GetValueString(filterData, "search_request");
        }

        private string GetValueString(Dictionary<string, string> filterData, string propertyName) {
            return filterData.ContainsKey(propertyName) ? filterData[propertyName] : string.Empty;
        }

        private List<string> GetValueArray(Dictionary<string, string> filterData, string propertyName) {
            var emptyFilters = new List<string>();
            if (filterData.ContainsKey(propertyName)) {
                var filters = filterData[propertyName];
                if (!string.IsNullOrEmpty(filters))
                    return filters.Split(';').ToList();
            }
            return emptyFilters;
        }
    }
}
