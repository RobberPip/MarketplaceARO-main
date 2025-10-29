namespace RegistryApp.Domain.Models;

public class Software {
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Description {
        get { return valueDescription; }
        set {
            valueDescription = value;
            flagDescription = true;
        }
    }
    public byte[]? Logo {
        get { return valueLogo; }
        set {
            valueLogo = value;
            flagLogo = true;
        }
    }
    public string? Vendor {
        get { return valueVendor; }
        set {
            valueVendor = value;
            flagVendor = true;
        }
    }
    public string? LinkTo {
        get { return valueLinkTo; }
        set {
            valueLinkTo = value;
            flagLinkTo = true;
        }
    }
    public Filters filter = new Filters();
    public int Rating { get; set; }
    public int ReviewCount { get; set; }
    public int LikeCount { get; set; }
    public bool IsLiked { get; set; }
    /// <summary>
    /// Данное свойство предназначенно для добавления/обновления значений атрибутов при помощи методов
    /// </summary>
    public Dictionary<string, object> Attributes { get; set; }
    /// <summary>
    /// Данное свойство введено как вспомогательное для вывода значений атрибутов, чтобы не использовать явное присовение строк к массиву (не делать лишних действий)
    /// </summary>
    public Dictionary<string, List<object>> AttributesFromOut {
        get {
            var transformedAttributes = new Dictionary<string, List<object>>();

            foreach (var pair in Attributes) {
                if (pair.Value is object[] values) {
                    transformedAttributes[pair.Key] = values.ToList();
                } else {
                    transformedAttributes[pair.Key] = new List<object> { pair.Value };
                }
            }

            return transformedAttributes;
        }
    }
    /// <summary>
    /// Словарь который хранит атрибуты, которые не пригодны для фильтрации, по умолчанию фильрация атрибутов задана как true
    /// </summary>
    public Dictionary<string, bool> FilterableAttributes { get; set; }

    /// <summary>
    /// Данные флаги нужны для того, чтобы мы могли присваивать null значения полям в БД
    /// </summary>
    #region flags
    private string valueDescription;
    public bool flagDescription = false;
    private byte[] valueLogo;
    public bool flagLogo = false;
    private string valueVendor;
    public bool flagVendor = false;
    private string valueLinkTo;
    public bool flagLinkTo = false;
    #endregion
    public Software() { }
    public List<Vendor> Vendors { get; set; }
    public Software(long id, string name, string description, string linkTo, byte[] logo, Dictionary<string, object> attributes) {
        Id = id;
        Name = name;
        Description = description;
        LinkTo = linkTo;
        Logo = logo;
        Attributes = attributes;
        Vendors = new List<Vendor>();
    }

}
