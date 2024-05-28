using System.Linq.Expressions;
using VehicleVault.Core.Settings;


namespace VehicleVault.Core.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> CreateAsync(T entity);
        T DeleteAsync(T entity);
        T UpdateAsync(T entity);
        Task<IEnumerable<T>> ReadAsync();
        Task<IEnumerable<T>> ReadAsync(Expression<Func<T, bool>> criteria);
        Task<IEnumerable<T>> ReadAsync(int skip, int take, Expression<Func<T, bool>> criteria = null!);
        Task<IEnumerable<T>> ReadAsync(Expression<Func<T, bool>> filter = null!, string[] includes1 = null!, string[] includes2 = null!, string[] includes3 = null!);
        Task<IEnumerable<T>> ReadAsync(Expression<Func<T, bool>> criteria, int? take, int? skip,
                                                   Expression<Func<T, object>> orderBy = null!, string orderByDirection = OrderBy.Ascending);
        Task<T?> GetByID(Expression<Func<T, bool>> criteria);
        Task<T> GetByID(Expression<Func<T, bool>> criteria, string[]? includes1 = null, string[]? includes2 = null, string[]? includes3 = null);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> criteria);
        Task<bool> IsValid(Expression<Func<T, bool>> criteria);
    }

}
