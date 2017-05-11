using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using ModernHttpClient;
using MovieDX.Core.Constants;
using MovieDX.Core.Helpers;
using MovieDX.Core.Interfaces;
using MovieDX.Core.Models;
using Newtonsoft.Json;

namespace MovieDX.Core
{
	public class TMDBMovieService : IMovieService
	{
		private Genres _genres = null;
		private Configuration _tmdbConfiguration;
		private HttpClient _baseClient;

        private HttpClient BaseClient
		{
			get
			{
				return _baseClient ?? (_baseClient = new HttpClient(new NativeMessageHandler())
				{
					BaseAddress = new Uri(AppConstants.TmdbBaseUrl)
				});
			}
		}

		private async Task GetConfigurationIfNeeded(bool force = false)
		{
			if (_tmdbConfiguration != null && !force) return;

			try
			{
				var res = await BaseClient.GetAsync(string.Format(AppConstants.TmdbConfigurationUrl, AppConstants.TmdbApiKey));
				res.EnsureSuccessStatusCode();

				var json = await res.Content.ReadAsStringAsync();

				if (string.IsNullOrEmpty(json)) throw new Exception("Return content was empty :(");

				_tmdbConfiguration = JsonConvert.DeserializeObject<Configuration>(json);
			}
			catch (Exception ex)
			{

				Debug.WriteLine(typeof(TMDBMovieService).Name,
					"Ooops! Something went wrong fetching the configuration. Exception: {1}", ex);
			}
		}

		public async Task<List<DetailedMovie>> SearchMovie(string movieTitle)
		{
			try
			{
				var res = await BaseClient.GetAsync(string.Format(AppConstants.TmdbMovieSearchUrl, AppConstants.TmdbApiKey,
					movieTitle));
				res.EnsureSuccessStatusCode();

				var json = await res.Content.ReadAsStringAsync();

				if (string.IsNullOrEmpty(json)) return null;

				var movies = JsonConvert.DeserializeObject<Movies>(json);
				await GetConfigurationIfNeeded();

				var movieList = movies.results.Where(x => x.original_title != null).Select(movie => new DetailedMovie
				{
					Id = movie.id,
					OriginalTitle = movie.original_title,
                    ComposedTitle = string.Format("{0}{1}({2})", movie.title.Substring(0, Math.Min(movie.title.Length, AppConstants.MovieTitleMaxLength)),
                                                  movie.title.Length > AppConstants.MovieTitleMaxLength
                                                  ? "..." : " ", string.IsNullOrEmpty(movie.release_date) ? "N/A" : movie.release_date.Substring(0, 4)),
                    Overview = movie.overview,
					Score = movie.vote_average,
					VoteCount = movie.vote_count,
					ImdbId = movie.imdb_id,
					PosterUrl = _tmdbConfiguration.images.base_url +
						_tmdbConfiguration.images.poster_sizes[3] +
						movie.poster_path,
					GenresCommaSeparated = GetGenresString(String.Join(", ", movie.genre_ids)),
					ReleaseDate = movie.release_date,
					Runtime = movie.runtime,
					Tagline = movie.tagline
				}).ToList();

				return movieList;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(typeof(TMDBMovieService).Name,
					"Ooops! Something went wrong fetching information for: {0}. Exception: {1}", movieTitle, ex);
				return null;
			}
		}

		public async Task<List<DetailedMovie>> DiscoverMovie(DiscoverOption option)
		{
			try
			{
				var dscUrl = string.Empty;

				if (_genres == null) GetGenres();

				switch (option)
				{
					case DiscoverOption.Popular:
						dscUrl = string.Format(AppConstants.TmdbMoviePopular, AppConstants.TmdbApiKey);
						break;
					case DiscoverOption.HighestRated:
						dscUrl = string.Format(AppConstants.TmdbMovieHighestRated, AppConstants.TmdbApiKey);
						break;
					case DiscoverOption.InTheaters:
						dscUrl = string.Format(AppConstants.TmdbMovieInTheaters, AppConstants.TmdbApiKey,
											   DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd"),
											   DateTime.Today.ToString("yyyy-MM-dd"));
						break;
					case DiscoverOption.Upcoming:
						dscUrl = string.Format(AppConstants.TmdbUpcomingMovies, AppConstants.TmdbApiKey);
						break;
					default:
						dscUrl = string.Format(AppConstants.TmdbMoviePopular, AppConstants.TmdbApiKey);
						break;
				}

				var res = await BaseClient.GetAsync(dscUrl);
				res.EnsureSuccessStatusCode();

				var json = await res.Content.ReadAsStringAsync();

				if (string.IsNullOrEmpty(json)) return null;

				var movies = JsonConvert.DeserializeObject<Movies>(json);
				await GetConfigurationIfNeeded();

				var movieList = movies.results.Select(movie => new DetailedMovie
				{
					Id = movie.id,
					OriginalTitle = movie.original_title,
					ComposedTitle = string.Format("{0}{1}({2})", movie.original_title.Substring(0, Math.Min(movie.original_title.Length, AppConstants.MovieTitleMaxLength)),
												  movie.original_title.Length > AppConstants.MovieTitleMaxLength
												  ? "..." : " ", movie.release_date.Substring(0, 4)),
					Overview = movie.overview,
					Score = movie.vote_average,
					VoteCount = movie.vote_count,
					ImdbId = movie.imdb_id,
					PosterUrl = _tmdbConfiguration.images.base_url +
						_tmdbConfiguration.images.poster_sizes[3] +
						movie.poster_path,
					GenresCommaSeparated = GetGenresString(String.Join(", ", movie.genre_ids)),
					ReleaseDate = movie.release_date,
					Runtime = movie.runtime,
					Tagline = movie.tagline
				}).ToList();

				return movieList;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(typeof(TMDBMovieService).Name,
								"Ooops! Something went wrong fetching information for: {0}. Exception: {1}", option.ToString(), ex);
				return null;
			}
		}


		public async Task<DetailedMovie> DetailedMovieFromId(int id)
		{
			try
			{
				var res = await BaseClient.GetAsync(string.Format(AppConstants.TmdbMovieUrl, AppConstants.TmdbApiKey,
					id));
				res.EnsureSuccessStatusCode();

				var json = await res.Content.ReadAsStringAsync();

				if (string.IsNullOrEmpty(json)) return null;

				var movie = JsonConvert.DeserializeObject<TmdbMovie>(json);
				await GetConfigurationIfNeeded();

				var detailed = new DetailedMovie
				{
					OriginalTitle = movie.original_title,
					ComposedTitle = string.Format("{0}{1}({2})", movie.original_title.Substring(0, Math.Min(movie.original_title.Length, AppConstants.MovieTitleMaxLength)),
					                              movie.original_title.Length > AppConstants.MovieTitleMaxLength 
					                              ? "..." : " ", movie.release_date.Substring(0, 4)),
					Overview = movie.overview,
					Score = movie.vote_average,
					VoteCount = movie.vote_count,
					ImdbId = movie.imdb_id,
					PosterUrl = _tmdbConfiguration.images.base_url +
						_tmdbConfiguration.images.poster_sizes[3] +
						movie.poster_path,
					GenresCommaSeparated = GetGenresString(String.Join(", ", movie.genre_ids)),
					ReleaseDate = movie.release_date,
					Runtime = movie.runtime,
					Tagline = movie.tagline
				};

				return detailed;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(typeof(TMDBMovieService).Name,
					"Ooops! Something went wrong fetching information id: {0}. Exception: {1}", id, ex);
				return null;
			}
		}

		private string GetGenresString(string genresIDs)
		{
			var r = string.Empty;

			try
			{
				var ids = genresIDs?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

				if (ids.Count() > 0)
				{
					int i = 0;
					foreach (var genreId in ids)
					{
						i++;

						var g = _genres.genres.FirstOrDefault(x => x.id == Int32.Parse(genreId));

						var genreName = (g != null) ? g.name : string.Empty;

						if (!string.IsNullOrEmpty(genreName))
						{
							if (g != null)
							{
								r = r + genreName;
							}

							if (i != ids.Count())
							{
								r = r + ", ";
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				ErrorLog.LogError("ERROR: Getting genres by ids", ex);
				r = string.Empty;
			}

			return r;
		}

		private void GetGenres()
		{
			_genres = new Genres();
			var result = string.Empty;
			try
			{
				var s = Assembly.Load(new AssemblyName("MovieDX.Core")).GetManifestResourceStream(@"MovieDX.Core.Extras.MovieGenres.json");
				var sr = new StreamReader(s);
				result = sr.ReadToEnd();

				_genres = Newtonsoft.Json.JsonConvert.DeserializeObject<Genres>(result);
			}
			catch (Exception ex)
			{
				ErrorLog.LogError("ERROR: Getting Genres", ex);
			}
		}
	}

}
