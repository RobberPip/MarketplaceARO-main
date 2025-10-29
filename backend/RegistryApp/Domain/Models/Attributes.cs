namespace RegistryApp.Domain.Models {
    public class Attributes {
        public long Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Свойство отвечающее за возможность фильрации атрибута, по умолчанию true
        /// </summary>
        /// <returns></returns>
        public bool FilterableAttribute { get; set; } = true;
        /// <summary>
        /// Кол-во зависимостей у атрибута
        /// </summary>
        public long DependentCount { get; set; }
        public List<string> AllScope { get; set; } = new List<string>();
        public Attributes() { }
        /// <summary>
        /// Данное свойство указывает на область действия атрибута, если оно будет указано не верно или вовсе не указано, то будет значение Global
        /// </summary>
        public string Scope { get; set; }
        public List<string> AttributesPossibleValues { get; set; }
        public Attributes(long id, string name, bool filterable, string scope, List<string> attributesPossibleValues) {
            Id = id;
            Name = name;
            FilterableAttribute = filterable;
            Scope = scope;
            AttributesPossibleValues = attributesPossibleValues;
        }
    }
}