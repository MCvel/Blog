using MovieDX.Core.Helpers;
using MovieDX.Core.Interfaces;
using MovieDX.Forms.DataAccess;
using MovieDX.Forms.Interfaces;
using MovieDX.Forms.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDX.Forms.ViewModels
{
    public class MovieListInfoPageViewModel : BaseViewModel
    {
        private readonly IRepository<Movie> _movieRepo;
        private CustomList _customList { get; set; }

		private string _listTitle;
		public string ListTitle
		{
			get { return _listTitle; }
			set { SetProperty(ref _listTitle, value); }
		}

        private List<MovieDX.Forms.Models.Movie> _moviesList;
        public List<MovieDX.Forms.Models.Movie> MoviesList
        {
            get { return _moviesList; }
            set { SetProperty(ref _moviesList, value); }
        }
        public MovieListInfoPageViewModel(IPageDialogService pageDialogService, INavigationService navigationService, IMovieService movieService)
            : base(pageDialogService, navigationService)
        {
            var connectionService = Xamarin.Forms.DependencyService.Get<ISQLite>();
            _movieRepo = new Repository<Movie>(connectionService);
        }

        private async Task LoadList()
        {
            try
            {
                var query = _movieRepo.AsQueryable();

                var movieList = await query.Where(x => x.ListId == _customList.id).ToListAsync();
                MoviesList = movieList;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError("Getting In Theater movies", ex);
            }
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
			try
			{
				_customList = (CustomList)parameters["list"];
				ListTitle = _customList.Name;
				Task.Run(LoadList).ConfigureAwait(true);
			}
			catch (Exception ex)
			{
				ErrorLog.LogError("ERROR: Loading list Movies", ex);
			}
        }
    }
}