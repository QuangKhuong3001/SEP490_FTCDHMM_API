using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SEP490_FTCDHMM_API.Shared.Utils
{

    public static class ViStringExtensions
    {
        public static string NormalizeVi(this string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            text = text.ToUpperInvariant().CleanDuplicateSpace();

            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var cat = CharUnicodeInfo.GetUnicodeCategory(c);
                if (cat != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            var noDiacritics = sb.ToString().Normalize(NormalizationForm.FormC);

            noDiacritics = noDiacritics.Replace('đ', 'd').Replace('Đ', 'd');

            return noDiacritics;
        }

        public static string CleanDuplicateSpace(this string name)
        {
            return Regex.Replace(name.Trim(), @"\s+", " ");
        }

        public static string UpperName(this string name)
        {
            return Regex.Replace(name.Trim().ToUpperInvariant(), @"\s+", " ");
        }

        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
