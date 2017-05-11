using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Services;
using Prism.Navigation;
using MovieDX.Core.Interfaces;
using MovieDX.Forms.Interfaces;
using MovieDX.Forms.DataAccess;
using System.Threading.Tasks;
using MovieDX.Forms.Models;
using MovieDX.Core.Helpers;

namespace MovieDX.Forms.ViewModels
{
	public class SeenMoviesPageViewModel : BaseViewModel
	{
		private readonly IMovieService _movieService;
		private readonly IRepository<Movie> _movieRepo;

		private List<MovieDX.Forms.Models.Movie> _seenMoviesList;
		public List<MovieDX.Forms.Models.Movie> SeenMoviesList
		{
			get { return _seenMoviesList; }
			set { SetProperty(ref _seenMoviesList, value); }
		}

		public SeenMoviesPageViewModel(IPageDialogService pageDialogService, INavigationService navigationService)
			: base(pageDialogService, navigationService)
		{
			var connectionService = Xamarin.Forms.DependencyService.Get<ISQLite>();
			_movieRepo = new Repository<Movie>(connectionService);

			Task.Run(LoadList).ConfigureAwait(true);
		}

		private async Task LoadList()
		{
			try
			{
				var query = _movieRepo.AsQueryable();

				SeenMoviesList = await query.Where(x => x.AlreadySeen == true).ToListAsync();
			}
			catch (Exception ex)
			{
				ErrorLog.LogError("Getting In Theater movies", ex);
			}
		}
	}
}