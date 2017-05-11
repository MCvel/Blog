using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using MovieDX.Forms.Droid;
using MovieDX.Forms.Interfaces;
using System.IO;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.XamarinAndroid;

[assembly: Dependency(typeof(SQLite_Android))]
namespace MovieDX.Forms.Droid
{
    class SQLite_Android : ISQLite
    {
        private SQLiteConnectionWithLock _conn;
        public void CloseConnection()
        {
            if (_conn != null)
            {
                _conn.Close();
                _conn.Dispose();
                _conn = null;

                // Must be called as the disposal of the connection is not released until the GC runs.
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public void DeleteDatabase()
        {
            var path = GetDatabasePath();

            try
            {
                if (_conn != null)
                {
                    _conn.Close();

                }
            }
            catch
            {
                // Best effort close. No need to worry if throws an exception
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            _conn = null;
        }

        private static string GetDatabasePath()
        {
            const string sqliteFilename = "moviedxdb.db3";

            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            var path = Path.Combine(documentsPath, sqliteFilename);

            return path;
        }

        public SQLiteAsyncConnection GetAsyncConnection()
        {
            var dbPath = GetDatabasePath();

            var platForm = new SQLitePlatformAndroid();

            var connectionFactory = new Func<SQLiteConnectionWithLock>(
                () =>
                {
                    if (_conn == null)
                    {
                        _conn =
                            new SQLiteConnectionWithLock(platForm,
                                new SQLiteConnectionString(dbPath, storeDateTimeAsTicks: true));
                    }
                    return _conn;
                });

            return new SQLiteAsyncConnection(connectionFactory);
        }

        public SQLiteConnection GetConnection()
        {
            var dbPath = GetDatabasePath();

            // Return the synchronous database connection 
            return new SQLiteConnection(new SQLitePlatformAndroid(), dbPath);
        }
    }
}