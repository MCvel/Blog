using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acr.UserDialogs;
using MovieDX.Core.Helpers;
using MovieDX.Core.Interfaces;
using MovieDX.Forms.DataAccess;
using MovieDX.Forms.Interfaces;
using MovieDX.Forms.Models;
using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Forms;
using Prism.Commands;
using MovieDX.Core.Models;
using Prism.Services;

namespace MovieDX.Forms.ViewModels
{
	public class MostPopularPageViewModel : BaseViewModel
	{
		private readonly IMovieService _movieService;
		private readonly IRepository<MovieDX.Forms.Models.Movie> _movieRepo;

        public DelegateCommand<DetailedMovie> AddWatchListCommand { get; set; }
        public DelegateCommand<DetailedMovie> AddSeenCommand { get; set; }
        public DelegateCommand<DetailedMovie> AddListCommand { get; set; }
        private DelegateCommand<ItemTappedEventArgs> _goToDetailPage;


        private List<MovieDX.Core.Models.DetailedMovie> _mostPopularList;
		public List<MovieDX.Core.Models.DetailedMovie> MostPopularList
		{
			get { return _mostPopularList; }
			set { SetProperty(ref _mostPopularList, value); }
		}

		private bool _isActive;

		public MostPopularPageViewModel(IPageDialogService pageDialogService, INavigationService navigationService, IMovieService movieService)
            : base(pageDialogService, navigationService)
        {
			_movieService = movieService;

			var connectionService = Xamarin.Forms.DependencyService.Get<ISQLite>();
			_movieRepo = new Repository<MovieDX.Forms.Models.Movie>(connectionService);

            AddWatchListCommand = new DelegateCommand<DetailedMovie>(async (DetailedMovie arg) => await AddToWatchList(arg));
            AddSeenCommand = new DelegateCommand<DetailedMovie>(async (DetailedMovie arg) => await AddToSeenList(arg));
            AddListCommand = new DelegateCommand<DetailedMovie>(async (DetailedMovie arg) => await AddToList(arg));

            Task.Run(LoadList).ConfigureAwait(true);
		}

		private async Task LoadList()
		{
			try
			{
				MostPopularList = await _movieService.DiscoverMovie(Core.Constants.DiscoverOption.Popular);
			}
			catch (Exception ex)
			{
				ErrorLog.LogError("Getting Highest rated movies", ex);
			}
		}

        public DelegateCommand<ItemTappedEventArgs> GoToDetailPage
        {
            get
            {
                if (_goToDetailPage == null)
                {
                    _goToDetailPage = new DelegateCommand<ItemTappedEventArgs>(async selected =>
                    {
                        var param = new NavigationParameters();
                        var movie = selected.Item as DetailedMovie;
                        param.Add("movie", movie);

                        await NavigateToUriWithModalOption(Constants.MovieDetailPageNoNav, param, false);
                    });
                }

                return _goToDetailPage;
            }
        }

        private async Task AddToWatchList(DetailedMovie detailedMovie)
        {
            try
            {
                var movieGet = await _movieRepo.Get(x => x.MovieId == detailedMovie.Id && x.ToWatch == true);

                if (movieGet == null)
                {
                    await _movieRepo.Insert(new MovieDX.Forms.Models.Movie
                    {
                        MovieName = detailedMovie.OriginalTitle,
                        ToWatch = true,
                        PosterURL = detailedMovie.PosterUrl,
                        MovieRate = detailedMovie?.Score == null ? "N/A" : detailedMovie?.Score.ToString(),
                        MovieDescription = detailedMovie.Overview,
                        MovieId = detailedMovie.Id,
                        DateAdded = DateTime.Now
                    });

                    await DisplayDialog("Info", "Added to WatchList", "Ok");
                }
                else
                {
                    await DisplayDialog("Info", "This Movie already exist in your watchlist", "Ok");
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError("Adding to Watchlist", ex);
            }
        }

        private async Task AddToSeenList(DetailedMovie detailedMovie)
        {
            try
            {
                await _movieRepo.Insert(new MovieDX.Forms.Models.Movie
                {
                    MovieName = detailedMovie.OriginalTitle,
                    AlreadySeen = true,
                    PosterURL = detailedMovie.PosterUrl,
                    MovieRate = detailedMovie?.Score == null ? "N/A" : detailedMovie?.Score.ToString(),
                    MovieDescription = detailedMovie.Overview,
                    MovieId = detailedMovie.Id,
                    DateAdded = DateTime.Now
                });

                await DisplayDialog("Info", "Added to Seen Movies", "Ok");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError("Adding to Seen list", ex);
            }
        }

        private async Task AddToList(DetailedMovie detailedMovie)
        {
            try
			{
				var param = new NavigationParameters();
				param.Add("movie", detailedMovie);

				await NavigateToUri(Constants.AddToListPage, param);
			}
			catch (Exception ex)
			{
				ErrorLog.LogError("Adding to list", ex);
			}
        }
        
        public bool IsActive
		{
			get
			{
				return _isActive;
			}

			set
			{
				_isActive = value;
				OnActiveTabChangedAsync();
			}
		}

		private async void OnActiveTabChangedAsync()
		{
			if (IsActive)
			{

			}
		}


	}
}
