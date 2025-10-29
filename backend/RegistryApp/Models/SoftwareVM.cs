using RegistryApp.Domain.Models;
using RegistryApp.Utils.CardShortLinks;
using RegistryApp.Utils.Helpers;

namespace RegistryApp.Models {
    public class SoftwareVM {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<VendorVM>? Vendors { get; set; }
        public string? LinkTo { get; set; }
        public string? ShortLink { get; set; }
        public string? SoftwareInstallationType { get; set; }
        public string? SoftwareActivityType { get; set; }
        public List<string>? ClassificationLifeStage { get; set; }
        public string? ClassificationContent { get; set; }
        public bool WorkWitnCIM { get; set; }
        public bool InHasNotim { get; set; }
        public bool RegRosreestr { get; set; }
        public int RegRosreestrNumber { get; set; }
        public string? RegRosreestrName { get; set; }
        public byte[]? Logo { get; set; }

        public SoftwareVM() { }
        public int Rating { get; set; }
        public int ReviewCount { get; set; }
        public int LikeCount { get; set; }
        public bool IsLiked { get; set; }
        public SoftwareVM(Software software) {
            Id = software.Id;
            Name = software.Name;
            Description = software.Description;
            Logo = software.Logo;
            Vendors = software.Vendors.Select(v => new VendorVM(v)).ToList();
            Rating = software.Rating;
            ReviewCount = software.ReviewCount;
            LikeCount = software.LikeCount;
            IsLiked = software.IsLiked;
        }

        public SoftwareVM(Software software, string hostname) {
            Id = software.Id;
            Name = software.Name;
            Description = software.Description;
            Vendors = software.Vendors.Select(v => new VendorVM(v)).ToList();
            LinkTo = software.LinkTo;
            Rating = software.Rating;
            ReviewCount = software.ReviewCount;
            LikeCount = software.LikeCount;
            IsLiked = software.IsLiked;

            ShortLink = $"{hostname}/cl/{CardShortLinks.GetCardLink(new CardDataDto() { SoftwareId = (int)software.Id })}" ?? string.Empty;

            SoftwareInstallationType = AttributesParser.DefineValueAttrributeAsString(software.Attributes, "Classification_Type");
            SoftwareActivityType = AttributesParser.DefineValueAttrributeAsString(software.Attributes, "Classification_Activity");
            ClassificationLifeStage = AttributesParser.DefineValueAttrributeAsStringArray(software.Attributes, "Classification_Stage");
            ClassificationContent = AttributesParser.DefineValueAttrributeAsString(software.Attributes, "Classification_Content");
            WorkWitnCIM = AttributesParser.DefineValueAttrributeAsBool(software.Attributes, "Compatibility_CIM");
            InHasNotim = AttributesParser.DefineValueAttrributeAsBool(software.Attributes, "IsHasNotim");
            RegRosreestr = AttributesParser.DefineValueAttrributeAsBool(software.Attributes, "IsHasReestr");
            RegRosreestrNumber = AttributesParser.DefineValueAttrributeAsInt(software.Attributes, "rr_reg_number");
            RegRosreestrName = AttributesParser.DefineValueAttrributeAsString(software.Attributes, "rr_name");
        }
    }
}