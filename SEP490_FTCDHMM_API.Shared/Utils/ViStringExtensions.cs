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

        public static int LevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
                return string.IsNullOrEmpty(t) ? 0 : t.Length;
            if (string.IsNullOrEmpty(t))
                return s.Length;

            var d = new int[s.Length + 1, t.Length + 1];

            for (int i = 0; i <= s.Length; i++)
                d[i, 0] = i;
            for (int j = 0; j <= t.Length; j++)
                d[0, j] = j;

            for (int i = 1; i <= s.Length; i++)
            {
                for (int j = 1; j <= t.Length; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost
                    );
                }
            }

            return d[s.Length, t.Length];
        }

        public static int SimilarityScore(string a, string b)
        {
            a = a ?? string.Empty;
            b = b ?? string.Empty;

            if (a == b) return 1000;
            if (b.Contains(a)) return 800;

            var dist = LevenshteinDistance(a, b);
            return -dist;
        }
    }
}
