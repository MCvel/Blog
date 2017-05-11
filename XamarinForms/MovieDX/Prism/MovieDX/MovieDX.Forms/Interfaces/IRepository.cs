using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Async;

namespace MovieDX.Forms.Interfaces
{
    public interface IRepository<T> where T : class, new()
    {
		Task<List<T>> GetAllAsync();
		Task<T> GetAsync(int id);
		Task<List<T>> Get<TValue>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, TValue>> orderBy = null);
		Task<T> Get(Expression<Func<T, bool>> predicate);
		AsyncTableQuery<T> AsQueryable();
		Task<int> Insert(T entity);
		Task<int> Update(T entity);
		Task<int> Delete(T entity);
		List<T> GetAll();
    }
}