using System.Globalization;
using System.Text;

namespace ProjectThoiTrang.Function
{
    public static class Function
    {
        public static string ToVND(this int? DonGia)
        {
            if (DonGia.HasValue)
            {
                return DonGia.Value.ToString("#,##0") + " đ";
            }
            else
            {
                return "0 đ";
            }
        }
        public static string FormatString(string s)
        {
            string normalString = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            foreach(var c in normalString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if(unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().Normalize(NormalizationForm.FormC).ToLower().Replace(" ", "");
        }
        public static string toTitleCase(string str)
        {
            string result = str;
            if (!string.IsNullOrEmpty(str))
            {
                var words = str.Split(' ');
                for(int index= 0; index < words.Length; index++)
                {
                    var s = words[index];
                    if(s.Length > 0)
                    {
                        words[index] = s[0].ToString().ToUpper() + s.Substring(1);
                    }
                }
                result = string.Join(" ", words);
            }
            return result;
        }

    }
}
