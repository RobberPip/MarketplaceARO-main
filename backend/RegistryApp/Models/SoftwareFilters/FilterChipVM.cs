namespace RegistryApp.Models.SoftwareFilters {
    public class FilterChipVM {
        public string FilterName { get; }
        public string FilterValue { get; }
        public bool IsSingleFilter { get; }
        public FilterChipVM(string name, string value, bool single = false) {
            FilterName = name;
            FilterValue = value;
            IsSingleFilter = single;
        }
    }
}
