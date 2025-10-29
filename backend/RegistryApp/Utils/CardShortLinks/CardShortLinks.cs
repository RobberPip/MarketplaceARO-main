using RegistryApp.Utils.Encoder;

namespace RegistryApp.Utils.CardShortLinks {
    public class CardShortLinks {
        public static string? GetCardLink(CardDataDto? cardData) {
            if (!cardData.HasValue)
                return null;

            byte[] bytes1 = BitConverter.GetBytes(cardData.Value.DeveloperId.GetValueOrDefault());
            byte[] bytes2 = BitConverter.GetBytes(cardData.Value.SoftwareId.GetValueOrDefault());

            int num = bytes1[2] != 0 || bytes1[3] != 0 || bytes2[2] != 0 || bytes2[3] != 0 ? 4 : 2;
            byte[] numArray = new byte[num * 2];

            Buffer.BlockCopy(bytes1, 0, numArray, 0, num);
            Buffer.BlockCopy(bytes2, 0, numArray, num, num);
            return Base32.ToBase32String(numArray)?.ToLower();
        }

        public static CardDataDto? GetCardData(string? shortLink) {
            if (string.IsNullOrEmpty(shortLink))
                return new CardDataDto?();

            byte[] src = Base32.FromBase32String(shortLink);
            if (src == null)
                return new CardDataDto?();

            int num = src.Length / 2;
            byte[] dst1 = new byte[4];
            byte[] dst2 = new byte[4];

            Buffer.BlockCopy(src, 0, dst1, 0, num);
            Buffer.BlockCopy(src, num, dst2, 0, num);

            return new CardDataDto?(new CardDataDto() {
                DeveloperId = new int?(BitConverter.ToInt32((ReadOnlySpan<byte>)dst1)),
                SoftwareId = new int?(BitConverter.ToInt32((ReadOnlySpan<byte>)dst2))
            });
        }
    }
}
