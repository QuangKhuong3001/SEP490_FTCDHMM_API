using System.Linq.Expressions;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> ListAsync();
        Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate);

        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);

        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}
