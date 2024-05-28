global using VehicleVault.Core.Interfaces;
using System.Data.Common;
using System.Linq.Expressions;
using VehicleVault.Core.Settings;
using VehicleVault.Ef.Data;

namespace VehicleVault.Ef.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T> CreateAsync(T entity)
        {
            try
            {
                await _context.Set<T>().AddAsync(entity);
                return entity;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Could not save entity", ex);
            }
            catch (Exception ex)
            {
                // Log generic exceptions here
                throw new Exception("An error occurred while creating the entity", ex);
            }
        }

        public T DeleteAsync(T entity)
        {
            try
            {
                _context.Set<T>().Remove(entity);
                return entity;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException("The entity was modified concurrently", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error deleting the entity", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the entity", ex);
            }
        }

        public T UpdateAsync(T entity)
        {
            try
            {
                _context.Update(entity);
                return entity;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException("The entity was modified concurrently", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error updating the entity", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the entity", ex);
            }
        }

        public async Task<IEnumerable<T>> ReadAsync()
        {
            try
            {
                return await _context.Set<T>().ToListAsync();
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException("Failed to retrieve data", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }
       

        public async Task<IEnumerable<T>> ReadAsync(Expression<Func<T, bool>> criteria)
        {
            try
            {
                return await _context.Set<T>().Where(criteria).ToListAsync();
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException("Failed to retrieve data with specified criteria", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

        public async Task<IEnumerable<T>> ReadAsync(int skip, int take, Expression<Func<T, bool>> criteria = null!)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>();

                if (criteria != null)
                {
                    query = query.Where(criteria);
                }

                return await query.Skip(skip).Take(take).ToListAsync();
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException("Failed to retrieve paginated data", ex);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("Invalid pagination parameters", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

        public async Task<IEnumerable<T>> ReadAsync(Expression<Func<T, bool>> filter = null!, string[] includes1 = null!, string[] includes2 = null!, string[] includes3 = null!)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>().Where(filter);

                var includes = (includes1 ?? Array.Empty<string>()).Concat(includes2 ?? Array.Empty<string>()).Concat(includes3 ?? Array.Empty<string>());

                foreach (var include in includes)
                {
                    query = query.Include(include);
                }

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                return await query.ToListAsync();
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException("Failed to retrieve data with includes", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving data", ex);
            }
        }

        public async Task<IEnumerable<T>> ReadAsync(Expression<Func<T, bool>> criteria, int? take, int? skip,
                                                   Expression<Func<T, object>> orderBy = null!, string orderByDirection = OrderBy.Ascending)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>().Where(criteria);

                if (skip.HasValue && skip.Value >= 0)
                {
                    query = query.Skip(skip.Value);
                }
                else if (skip.HasValue)
                {
                    throw new ArgumentException("Skip value must be non-negative");
                }

                if (take.HasValue && take.Value > 0)
                {
                    query = query.Take(take.Value);
                }
                else if (take.HasValue)
                {
                    throw new ArgumentException("Take value must be positive");
                }

                if (orderBy != null)
                {
                    query = orderByDirection.ToLower() == "ascending" ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
                }

                return await query.ToListAsync();
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException("Failed to retrieve data with specified criteria and pagination", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving data", ex);
            }
        }

        public async Task<T?> GetByID(Expression<Func<T, bool>> criteria)
        {
            try
            {
                return await _context.Set<T>().SingleOrDefaultAsync(criteria);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error retrieving data by ID.", ex);
            }
        }

        public async Task<T> GetByID(Expression<Func<T, bool>> criteria, string[]? includes1 = null, string[]? includes2 = null, string[]? includes3 = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes1 != null)
            {
                foreach (var include in includes1)
                {
                    query = query.Include(include);
                }
            }

            if (includes2 != null)
            {
                foreach (var include in includes2)
                {
                    query = query.Include(include);
                }
            }
            if (includes3 != null)
            {
                foreach (var include in includes3)
                {
                    query = query.Include(include);
                }
            }
            var result = await query.SingleOrDefaultAsync(criteria);
            if (result == null)
            {
                throw new InvalidOperationException("No entity found with the specified criteria.");
            }
            return result;
        }

        public async Task<int> CountAsync()
        {
            try
            {
                return await _context.Set<T>().CountAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error counting entities.", ex);
            }
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> criteria)
        {
            try
            {
                return await _context.Set<T>().CountAsync(criteria);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error counting entities with criteria.", ex);
            }
        }

        public async Task<bool> IsValid(Expression<Func<T, bool>> criteria)
        {
            try
            {
                return await _context.Set<T>().AnyAsync(criteria);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error validating entity existence.", ex);
            }
        }





    }
}
