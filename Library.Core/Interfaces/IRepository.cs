using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Library.Core.Interfaces
{
    public interface IRepository<T>where T : class
    {
        Task<T?> GetById(int id);
        Task<IEnumerable<T>> GetAllAsync(T entity);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T,bool>> predicate);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);

        // Check if any entity matches condition
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        // Count entities (optionally with condition)
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);


    }
}
