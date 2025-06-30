using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        T GetById(int id);
        Task<T> GetByIdAsync(int id);

        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync();

        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);

        IQueryable<T> GetQueryable();

        T SingleOrDefault(Expression<Func<T, bool>> expression);
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> expression);

        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");

        void Add(T entity);
        Task AddAsync(T entity);

        void AddRange(IEnumerable<T> entities);
        Task AddRangeAsync(IEnumerable<T> entities);

        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        void Update(T entity);

        bool Exists(Expression<Func<T, bool>> expression);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> expression);

        int Count(Expression<Func<T, bool>> expression = null);
        Task<int> CountAsync(Expression<Func<T, bool>> expression = null);
    }
}
