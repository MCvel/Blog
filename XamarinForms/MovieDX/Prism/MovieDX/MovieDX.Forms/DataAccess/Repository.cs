using MovieDX.Forms.Interfaces;
using SQLite.Net.Async;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MovieDX.Forms.Helpers;
using System.Linq.Expressions;

namespace MovieDX.Forms.DataAccess
{
	public class Repository<T> : IRepository<T> where T : class, new()
	{
		ISQLite _connectionService;
		SQLiteAsyncConnection _db;
		private readonly AsyncLock Locker = new AsyncLock();

		public Repository(ISQLite connectionService)
		{
			_connectionService = connectionService;
			_db = _connectionService.GetAsyncConnection();
			_db.CreateTableAsync<T>();
		}

		public AsyncTableQuery<T> AsQueryable() =>
			_db.Table<T>();

		public async Task<List<T>> GetAllAsync() =>
			await _db.Table<T>().ToListAsync();

		public async Task<List<T>> Get<TValue>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, TValue>> orderBy = null)
		{
			var query = _db.Table<T>();

			if (predicate != null)
				query = query.Where(predicate);

			if (orderBy != null)
				query = query.OrderBy<TValue>(orderBy);

			return await query.ToListAsync();
		}

		public async Task<T> GetAsync(int id) =>
			 await _db.FindAsync<T>(id);

		public async Task<T> Get(Expression<Func<T, bool>> predicate) =>
			await _db.FindAsync<T>(predicate);

		public async Task<int> Insert(T entity)
		{
			using (await Locker.LockAsync())
			{
				return await _db.InsertAsync(entity);
			}
		}

		public async Task<int> Update(T entity)
		{
			using (await Locker.LockAsync())
			{
				return await _db.UpdateAsync(entity);
			}
		}

		public async Task<int> Delete(T entity)
		{
			using (await Locker.LockAsync())
			{
				return await _db.DeleteAsync(entity);
			}
		}

		public List<T> GetAll()
		{
            var list = _db.Table<T>();

            var result = list.ToListAsync().Result;

            return result;
        }
	}
}