using SQLite.Net;
using SQLite.Net.Async;

namespace MovieDX.Forms.Interfaces
{
    public interface ISQLite
    {
        void CloseConnection();
        SQLiteConnection GetConnection();
        SQLiteAsyncConnection GetAsyncConnection();
        void DeleteDatabase();
    }
}