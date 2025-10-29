using System.Text;

namespace RegistryApp.Utils.Encoder {
    public class Base32 {
        private static readonly char[] _digits = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();
        private const int _mask = 31;
        private const int _shift = 5;

        public static byte[]? FromBase32String(string encoded) {
            if (encoded == null)
                return null;
            encoded = encoded.Trim().TrimEnd('=').ToUpper();
            if (encoded.Length == 0)
                return null;

            byte[] numArray = new byte[encoded.Length * 5 / 8];
            int num1 = 0;
            int num2 = 0;
            int num3 = 0;

            foreach (char c in encoded) {
                int num4 = Base32.CharToInt(c);
                if (num4 < 0)
                    return null;
                num1 = num1 << 5 | num4 & 31;
                num3 += 5;
                if (num3 >= 8) {
                    numArray[num2++] = (byte)(num1 >> num3 - 8);
                    num3 -= 8;
                }
            }
            return numArray;
        }

        public static string? ToBase32String(byte[] data, bool padOutput = false) => Base32.ToBase32String(data, 0, data.Length, padOutput)?.ToLower();

        private static string? ToBase32String(byte[] data, int offset, int length, bool padOutput = false) {
            if (data == null)
                return null;
            if (offset < 0)
                return null;
            if (length < 0)
                return null;
            if (offset + length > data.Length)
                return null;
            if (length == 0)
                return string.Empty;
            if (length >= 268435456)
                return null;

            StringBuilder stringBuilder = new StringBuilder((length * 8 + 5 - 1) / 5);
            int num1 = offset + length;
            int num2 = data[offset++];
            int num3 = 8;

            while (num3 > 0 || offset < num1) {
                if (num3 < 5) {
                    if (offset < num1) {
                        num2 = num2 << 8 | (int)data[offset++] & (int)byte.MaxValue;
                        num3 += 8;
                    } else {
                        int num4 = 5 - num3;
                        num2 <<= num4;
                        num3 += num4;
                    }
                }
                int index = 31 & num2 >> num3 - 5;
                num3 -= 5;
                stringBuilder.Append(Base32._digits[index]);
            }
            if (padOutput) {
                int num5 = 8 - stringBuilder.Length % 8;
                if (num5 > 0)
                    stringBuilder.Append('=', num5 == 8 ? 0 : num5);
            }
            return stringBuilder.ToString();
        }

        private static int CharToInt(char c) {
            int num;
            switch (c) {
                case '2':
                    num = 26;
                    break;
                case '3':
                    num = 27;
                    break;
                case '4':
                    num = 28;
                    break;
                case '5':
                    num = 29;
                    break;
                case '6':
                    num = 30;
                    break;
                case '7':
                    num = 31;
                    break;
                case 'A':
                    num = 0;
                    break;
                case 'B':
                    num = 1;
                    break;
                case 'C':
                    num = 2;
                    break;
                case 'D':
                    num = 3;
                    break;
                case 'E':
                    num = 4;
                    break;
                case 'F':
                    num = 5;
                    break;
                case 'G':
                    num = 6;
                    break;
                case 'H':
                    num = 7;
                    break;
                case 'I':
                    num = 8;
                    break;
                case 'J':
                    num = 9;
                    break;
                case 'K':
                    num = 10;
                    break;
                case 'L':
                    num = 11;
                    break;
                case 'M':
                    num = 12;
                    break;
                case 'N':
                    num = 13;
                    break;
                case 'O':
                    num = 14;
                    break;
                case 'P':
                    num = 15;
                    break;
                case 'Q':
                    num = 16;
                    break;
                case 'R':
                    num = 17;
                    break;
                case 'S':
                    num = 18;
                    break;
                case 'T':
                    num = 19;
                    break;
                case 'U':
                    num = 20;
                    break;
                case 'V':
                    num = 21;
                    break;
                case 'W':
                    num = 22;
                    break;
                case 'X':
                    num = 23;
                    break;
                case 'Y':
                    num = 24;
                    break;
                case 'Z':
                    num = 25;
                    break;
                default:
                    num = -1;
                    break;
            }
            return num;
        }
    }
}
