using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MovieDX.Core.Constants;
using MovieDX.Core.Models;

namespace MovieDX.Core.Interfaces
{
	public interface IMovieService
	{
		Task<List<DetailedMovie>> SearchMovie(string movieTitle);
		Task<DetailedMovie> DetailedMovieFromId(int id);
		Task<List<DetailedMovie>> DiscoverMovie(DiscoverOption option);
	}
}
