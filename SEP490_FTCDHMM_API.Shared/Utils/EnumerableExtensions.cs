namespace SEP490_FTCDHMM_API.Shared.Utils
{
    public static class EnumerableExtensions
    {
        public static bool HasDuplicate<T>(this IEnumerable<T> list)
        {
            return list.GroupBy(x => x).Any(g => g.Count() > 1);
        }
    }
}
