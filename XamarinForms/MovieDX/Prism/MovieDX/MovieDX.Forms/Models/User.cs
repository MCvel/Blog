using System;
using SQLite.Net.Attributes;

namespace MovieDX.Forms.Models
{
	public class User
	{
		[PrimaryKey, AutoIncrement]
		public int id { get; set; }
		public string Email { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public bool IsLoggedIn { get; set; }
		public DateTime DateAdded { get; set; }
	}
}