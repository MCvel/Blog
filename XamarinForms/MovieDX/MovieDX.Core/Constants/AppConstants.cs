using System;
using System.Collections.Generic;

namespace MovieDX.Core.Constants
{
	internal class AppConstants
	{
		public const string TmdbApiKey = "c0d0458190688fc0796d41acf1cef8ec";
		public const string TmdbBaseUrl = "https://api.themoviedb.org/3/";
		public const string TmdbMovieSearchUrl = "search/multi?api_key={0}&query={1}";
		public const string TmdbMovieInTheaters = "discover/movie?api_key={0}&primary_release_date.gte={1}&primary_release_date.lte={2}";
		public const string TmdbMoviePopular = "discover/movie?api_key={0}&sort_by=popularity.desc";
		public const string TmdbMovieHighestRated = "discover/movie?api_key={0}&certification_country=US&certification=R&sort_by=vote_average.desc";
		public const string TmdbConfigurationUrl = "configuration?api_key={0}";
		public const string TmdbMovieUrl = "movie/{1}?api_key={0}";
		public const string TmdbSimilarMovies = "movie/{0}/similar?api_key={1}&language=en-US&page=1";
		public const string TmdbMovieCredits = "movie/{0}/credits?api_key={1}";
		public const string TmdbUpcomingMovies = "movie/upcoming?api_key={0}&language=en-US&page=1";

		public const int MovieTitleMaxLength = 20;
        
 	}

	public enum DiscoverOption
	{
		InTheaters,
		Popular,
		HighestRated,
		Upcoming
	}
}