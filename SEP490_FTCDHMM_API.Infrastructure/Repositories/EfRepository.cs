using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _dbContext;

        public EfRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
        }
        public async Task<T?> GetByIdAsync(Guid id, Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
        }

        public async Task<IList<T>> GetAllAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (predicate != null)
                query = query.Where(predicate);

            if (include != null)
                query = include(query);

            return await query.ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbContext.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate == null
                ? await _dbContext.Set<T>().CountAsync()
                : await _dbContext.Set<T>().CountAsync(predicate);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().AnyAsync(predicate);
        }

        public async Task<bool> IdsExistAsync(List<Guid> ids)
        {
            if (ids == null || ids.Count == 0)
                return false;

            var parameter = Expression.Parameter(typeof(T), "e");
            var property = Expression.PropertyOrField(parameter, "Id");
            var containsMethod = typeof(List<Guid>).GetMethod(nameof(List<Guid>.Contains), new[] { typeof(Guid) })!;
            var containsCall = Expression.Call(Expression.Constant(ids), containsMethod, property);
            var lambda = Expression.Lambda<Func<T, bool>>(containsCall, parameter);

            var count = await _dbContext.Set<T>().CountAsync(lambda);
            return count == ids.Count;
        }

        public async Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? keyword = null,
            string[]? searchProperties = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (include != null)
                query = include(query);

            if (filter != null)
                query = query.Where(filter);

            if (!string.IsNullOrWhiteSpace(keyword) && searchProperties != null && searchProperties.Length > 0)
            {
                var parameter = Expression.Parameter(typeof(T), "e");
                Expression? combined = null;

                foreach (var prop in searchProperties)
                {
                    var property = Expression.PropertyOrField(parameter, prop);
                    if (property.Type == typeof(string))
                    {
                        var contains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;
                        var keywordConst = Expression.Constant(keyword, typeof(string));
                        var call = Expression.Call(property, contains, keywordConst);
                        combined = combined == null ? call : Expression.OrElse(combined, call);
                    }
                }

                if (combined != null)
                {
                    var lambda = Expression.Lambda<Func<T, bool>>(combined, parameter);
                    query = query.Where(lambda);
                }
            }

            if (orderBy != null)
                query = orderBy(query);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }



    }
}
