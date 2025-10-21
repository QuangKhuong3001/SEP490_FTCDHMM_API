using System.Globalization;
using System.Text;

namespace SEP490_FTCDHMM_API.Shared.Utils
{

    public static class ViStringExtensions
    {
        public static string NormalizeVi(this string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            text = text.Trim().ToLowerInvariant();

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
    }
}
