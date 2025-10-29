using RegistryApp.Domain.Models;
using RegistryApp.Utils.CardShortLinks;
using RegistryApp.Utils.Helpers;

namespace RegistryApp.Models {
    public class VendorVM {
        public long Id { get; set; }
        public string? FullName { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? LinkTo { get; set; }
        public string? ShortLink { get; set; }
        public string? VendorActivity { get; set; }
        public bool InNOTIM { get; set; }
        public double Rating { get; set; }
        public string? DateToAdd { get; set; }
        public int CountSoftwaresInRosReestr { get; set; }
        public byte[]? Logo { get; set; }

        public VendorVM() { }

        public VendorVM(Vendor vendor) {
            Id = vendor.Id;
            Name = vendor.Name;
            FullName = vendor.OtherName;
            Description = vendor.Description;
        }

        public VendorVM(Vendor vendor, string hostname, int countInRosReestr) {
            Id = vendor.Id;
            Name = vendor.Name;
            FullName = vendor.OtherName ?? vendor.Name;
            LinkTo = vendor.LinkTo ?? "Неопределенно";

            ShortLink = $"{hostname}/cl/{CardShortLinks.GetCardLink(new CardDataDto() { DeveloperId = (int)vendor.Id })}" ?? string.Empty;

            Logo = vendor.Logo;
            Description = vendor.Description ?? "Описание отсутствует";
            DateToAdd = "хх.хх.хххх";
            VendorActivity = "Неопределенно";
            InNOTIM = AttributesParser.DefineValueAttrributeAsBool(vendor.Attributes, "IsHasNotim");
            CountSoftwaresInRosReestr = countInRosReestr;
        }
    }
}
