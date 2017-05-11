using System;
using SQLite.Net.Attributes;

namespace MovieDX.Forms.Models
{
	public class Movie
	{
		[PrimaryKey, AutoIncrement]
		public int id { get; set; }
		public int MovieId { get; set; }
        public int ListId { get; set; }
		public string MovieName { get; set; }
		public string PosterURL { get; set; }
		public string MovieDescription { get; set; }
		public string MovieRate { get; set; }
		public string MovieActors { get; set; }
        public string Genres { get; set; }
        public bool ToWatch { get; set; } = false;
		public bool AlreadySeen { get; set; } = false;
		public DateTime DateAdded { get; set; }
	}
}