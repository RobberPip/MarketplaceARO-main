namespace RegistryApp.Utils.CardShortLinks {
    public struct CardDataDto {
        public int? DeveloperId;
        public int? SoftwareId;

        public CardDataDto() {
            this.DeveloperId = new int?();
            this.SoftwareId = new int?();
        }
    }
}
