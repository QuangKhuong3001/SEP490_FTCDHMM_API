using System.Linq.Expressions;

namespace SEP490_FTCDHMM_API.Application.Interfaces.Persistence
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes);
        Task<T?> GetByIdAsync(Guid id, Func<IQueryable<T>, IQueryable<T>>? include = null);
        Task<IList<T>> GetAllAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null);

        Task SaveChangeAsync();
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entities);

        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<bool> IdsExistAsync(List<Guid> ids);
        Task<bool> IdsExistAsync(IEnumerable<Guid> ids);
        Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? keyword = null,
            string[]? searchProperties = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null);

        Task<T?> GetLatestAsync<TKey>(
            Expression<Func<T, TKey>> orderByDescendingKeySelector,
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null);
    }
}
