using System.Linq;
using System.Text.RegularExpressions;

namespace CartridgeStore.Helpers
{
    public enum BarCodeType
    {
        Unknown,
        EAN8,
        EAN13,
        GTIN,
        UPC
    }

    public static class BarCodeHelper
    {
        private static BarCodeType AnalyzeArticleNumberType(string code)
        {
            BarCodeType barCodeType = BarCodeType.Unknown;

            if (!long.TryParse(code, out long temp))
            {
                return barCodeType;
            }

            switch (code.Length)
            {
                case 8:
                    barCodeType = BarCodeType.EAN8;
                    break;
                case 12:
                    barCodeType = BarCodeType.UPC;
                    break;
                case 13:
                    barCodeType = BarCodeType.EAN13;
                    break;
                case 14:
                    barCodeType = BarCodeType.GTIN;
                    break;
                default:
                    return barCodeType;
            }

            code = $"{temp:00000000000000}";

            int[] a = new int[13];
            a[0]  = (code[0] - '0') * 3;
            a[1]  = code[1] - '0';
            a[2]  = (code[2] - '0') * 3;
            a[3]  = code[3] - '0';
            a[4]  = (code[4] - '0') * 3;
            a[5]  = code[5] - '0';
            a[6]  = (code[6] - '0') * 3;
            a[7]  = code[7] - '0';
            a[8]  = (code[8] - '0') * 3;
            a[9]  = code[9] - '0';
            a[10] = (code[10] - '0') * 3;
            a[11] = code[11] - '0';
            a[12] = (code[12] - '0') * 3;

            int sum   = a.Sum();
            int check = (10 - sum % 10) % 10;
            return check == code[13] - '0' ? barCodeType : BarCodeType.Unknown;
        }

        public static bool IsValidBarCode(string code)
        {
            return IsValidAsin(code) || IsValidEan(code) || IsValidGtin(code) || IsValidUpc(code);
        }

        private static bool IsValidAsin(string asin)
        {
            if (string.IsNullOrEmpty(asin))
            {
                return false;
            }

            if (asin.Length != 10)
            {
                return false;
            }

            Regex regex = new Regex("^B\\d{2}\\w{7}|\\d{9}(X|\\d)$");
            return regex.IsMatch(asin);
        }

        private static bool IsValidEan(string code)
        {
            BarCodeType barCodeType = AnalyzeArticleNumberType(code);
            return barCodeType == BarCodeType.EAN8 || barCodeType == BarCodeType.EAN13;
        }

        private static bool IsValidUpc(string code)
        {
            BarCodeType barCodeType = AnalyzeArticleNumberType(code);
            return barCodeType == BarCodeType.UPC;
        }

        private static bool IsValidGtin(string code)
        {
            BarCodeType barCodeType = AnalyzeArticleNumberType(code);
            return barCodeType == BarCodeType.GTIN;
        }
    }
}