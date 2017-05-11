using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Services;
using Prism.Navigation;
using MovieDX.Core.Interfaces;
using MovieDX.Forms.Interfaces;
using MovieDX.Forms.Models;
using System.Threading.Tasks;
using MovieDX.Forms.DataAccess;
using MovieDX.Core.Helpers;

namespace MovieDX.Forms.ViewModels
{
	public class WatchListPageViewModel : BaseViewModel
	{
		private readonly IMovieService _movieService;
		private readonly IRepository<Movie> _movieRepo;

		private List<MovieDX.Forms.Models.Movie> _toWatchList;
		public List<MovieDX.Forms.Models.Movie> ToWatchList
		{
			get { return _toWatchList; }
			set { SetProperty(ref _toWatchList, value); }
		}

		public WatchListPageViewModel(IPageDialogService pageDialogService, INavigationService navigationService)
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

				ToWatchList = await query.Where(x => x.ToWatch == true).ToListAsync();
			}
			catch (Exception ex)
			{
				ErrorLog.LogError("Getting In Theater movies", ex);
			}
		}
	}
}