namespace RegistryApp.Domain.Models;

public class Vendor {
    public long Id { get; set; }
    public string? Name { get; set; }

    public string? Description {
        get { return valueDescription; }
        set {
            valueDescription = value;
            flagDescription = true;
        }
    }
    public string? LinkTo {
        get { return valueLinkTo; }
        set {
            valueLinkTo = value;
            flagLinkTo = true;
        }
    }
    public string? OtherName {
        get { return valueOtherName; }
        set {
            valueOtherName = value;
            flagOtherName = true;
        }
    }
    public byte[]? Logo {
        get { return valueLogo; }
        set {
            valueLogo = value;
            flagLogo = true;
        }
    }
    public string? PublicName {
        get { return valuePublicName; }
        set {
            valuePublicName = value;
            flagPublicName = true;
        }
    }
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
    private string valueLinkTo;
    public bool flagLinkTo = false;
    private string valueOtherName;
    public bool flagOtherName = false;
    private string valuePublicName;
    public bool flagPublicName = false;
    private byte[] valueLogo;
    public bool flagLogo = false;
    #endregion

    public Vendor() { }

    public Vendor(long id, string name, string description, string linkTo, string otherName, string publicName, Dictionary<string, object> attributes, byte[] logo) {
        Id = id;
        Name = name;
        Description = description;
        LinkTo = linkTo;
        OtherName = otherName;
        PublicName = publicName;
        Attributes = attributes;
        Logo = logo;
    }
}