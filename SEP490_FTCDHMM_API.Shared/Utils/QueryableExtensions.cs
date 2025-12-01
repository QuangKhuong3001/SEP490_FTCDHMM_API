using System.Linq.Expressions;

namespace SEP490_FTCDHMM_API.Shared.Utils
{
    public static class QueryableExtensions
    {
        public static IOrderedQueryable<T> OrderByKeyword<T>(
            this IQueryable<T> query,
            string? keyword,
            params Expression<Func<T, string?>>[] properties)
        {
            if (string.IsNullOrWhiteSpace(keyword) || properties.Length == 0)
            {
                var lastUpdatedProp = typeof(T).GetProperty("LastUpdatedUtc");
                if (lastUpdatedProp != null)
                {
                    var parameter = Expression.Parameter(typeof(T), "x");
                    var propertyAccess = Expression.Property(parameter, lastUpdatedProp);
                    var lambda = Expression.Lambda<Func<T, DateTime>>(propertyAccess, parameter);
                    return query.OrderByDescending(lambda);
                }

                return (IOrderedQueryable<T>)query;
            }

            keyword = keyword.ToLower();

            IOrderedQueryable<T> ordered = query.OrderBy(x => 0);

            foreach (var prop in properties)
            {
                var param = prop.Parameters[0];
                var toLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
                var loweredProp = Expression.Call(prop.Body, toLower);

                var keywordConst = Expression.Constant(keyword);

                var eq = Expression.Equal(loweredProp, keywordConst);
                var eqLambda = Expression.Lambda<Func<T, bool>>(eq, param);

                var startsWith = typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string) })!;
                var startsWithCall = Expression.Call(loweredProp, startsWith, keywordConst);
                var startsWithLambda = Expression.Lambda<Func<T, bool>>(startsWithCall, param);

                var contains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;
                var containsCall = Expression.Call(loweredProp, contains, keywordConst);
                var containsLambda = Expression.Lambda<Func<T, bool>>(containsCall, param);

                if (ordered == null)
                {
                    ordered = query
                        .OrderByDescending(eqLambda)
                        .ThenByDescending(startsWithLambda)
                        .ThenByDescending(containsLambda);
                }
                else
                {
                    ordered = ordered
                        .ThenByDescending(eqLambda)
                        .ThenByDescending(startsWithLambda)
                        .ThenByDescending(containsLambda);
                }
            }

            var lastUpdated = typeof(T).GetProperty("LastUpdatedUtc");
            if (lastUpdated != null)
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var propertyAccess = Expression.Property(parameter, lastUpdated);
                var lambda = Expression.Lambda<Func<T, DateTime>>(propertyAccess, parameter);
                ordered = ordered.ThenByDescending(lambda);
            }

            return ordered ?? (IOrderedQueryable<T>)query;
        }

        public static IOrderedQueryable<T> ThenByKeyword<T>(
        this IOrderedQueryable<T> query,
        string? keyword,
        params Expression<Func<T, string?>>[] properties)
        {
            if (string.IsNullOrWhiteSpace(keyword) || properties.Length == 0)
                return query;

            keyword = keyword.ToLower();

            foreach (var prop in properties)
            {
                var param = prop.Parameters[0];

                var toLower = typeof(string)
                    .GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
                var loweredProp = Expression.Call(prop.Body, toLower);

                var keywordConst = Expression.Constant(keyword);

                var eq = Expression.Equal(loweredProp, keywordConst);
                var eqLambda = Expression.Lambda<Func<T, bool>>(eq, param);

                var startsWith = typeof(string)
                    .GetMethod(nameof(string.StartsWith), new[] { typeof(string) })!;
                var startsWithCall = Expression.Call(loweredProp, startsWith, keywordConst);
                var startsWithLambda = Expression.Lambda<Func<T, bool>>(startsWithCall, param);

                var contains = typeof(string)
                    .GetMethod(nameof(string.Contains), new[] { typeof(string) })!;
                var containsCall = Expression.Call(loweredProp, contains, keywordConst);
                var containsLambda = Expression.Lambda<Func<T, bool>>(containsCall, param);

                query = query
                    .ThenByDescending(eqLambda)
                    .ThenByDescending(startsWithLambda)
                    .ThenByDescending(containsLambda);
            }

            return query;
        }
    }

}
