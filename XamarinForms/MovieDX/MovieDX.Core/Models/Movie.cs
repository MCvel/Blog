using System;
namespace MovieDX.Core.Models
{
	public class Movie
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string PosterUrl { get; set; }
		public double Score { get; set; }
	}
}
