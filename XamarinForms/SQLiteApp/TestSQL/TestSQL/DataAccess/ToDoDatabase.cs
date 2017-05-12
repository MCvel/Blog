using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using TestSQL.DataAccess;
using TestSQL.Models;

namespace TestSQL.DataAccess
{
	public class ToDoDatabase
	{
		SQLiteConnection database { get; set;}
		public static string Root { get; set; } 
		static object locker = new object();

		public ToDoDatabase ()
		{
			
			var location = "tododb.db3";
			location = System.IO.Path.Combine (Root, location);
			database = new SQLite.SQLiteConnection (location);

			database.CreateTable<ToDoItem> ();
		}

		public IEnumerable<T> GetItems<T>() where T : IBusinessEntity, new()
		{
			lock (locker)
			{
				return (from i in database.Table<T>() select i).ToList();
			}
		}

		public T GetItem<T>(int id) where T : IBusinessEntity, new()
		{
			lock (locker)
			{
				return database.Table<T>().FirstOrDefault(x => x.Id == id);
			}
		}

		public int SaveItem<T>(T item) where T : IBusinessEntity
		{
			lock (locker)
			{
				if (item.Id != 0)
				{
					database.Update(item);
					return item.Id;
				}
				else
				{
					return database.Insert(item);
				}
			}
		}

		public int DeleteItem<T>(int id) where T : IBusinessEntity, new()
		{
			lock (locker)
			{
				return database.Delete<T>(new T() { Id = id });
			}
		}
	}
}