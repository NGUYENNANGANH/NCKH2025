using System.Linq.Expressions;

namespace BanHang.API.Data.Repositories
{
    public interface IRepository<T> where T : class
    {
        // Read operations
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        // Create operations
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        // Update operations
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);

        // Delete operations
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        // Queries
        IQueryable<T> Query();
    }
} 