using MovieDX.Core.Helpers;
using MovieDX.Core.Models;
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
using Xamarin.Forms;

namespace MovieDX.Forms.ViewModels
{
    public class AddToListPageViewModel : BaseViewModel
    {
        private readonly IRepository<CustomList> _listRepo;
        private readonly IRepository<MovieDX.Forms.Models.Movie> _movieRepo;
        private DelegateCommand<ItemTappedEventArgs> _addToListCommand;
        private Command _createListCommand;

        public bool HasData { get; set; } = false;

        private DetailedMovie _selectedMovie;

        private string _newList;
        public string NewList
        {
            get { return _newList; }
            set { SetProperty(ref _newList, value); }
        }

        private List<CustomList> _movieList;
        public List<CustomList> MovieList
        {
            get { return _movieList; }
            set { SetProperty(ref _movieList, value); }
        }

        public AddToListPageViewModel(IPageDialogService pageDialogService, INavigationService navigationService)
            : base(pageDialogService, navigationService)
        {
            var connectionService = Xamarin.Forms.DependencyService.Get<ISQLite>();
            _listRepo = new Repository<CustomList>(connectionService);
            _movieRepo = new Repository<MovieDX.Forms.Models.Movie>(connectionService);

            Task.Run(LoadList).ConfigureAwait(true);
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            _selectedMovie = (DetailedMovie)parameters["movie"];
        }

        public override void OnNavigatedFrom(NavigationParameters parameters)
        {
            _selectedMovie = (DetailedMovie)parameters["movie"];
        }

        public DelegateCommand<ItemTappedEventArgs> AddToListCommand
        {
            get
            {
                if (_addToListCommand == null)
                {
                    _addToListCommand = new DelegateCommand<ItemTappedEventArgs>(async selected =>
                    {
                        var list = selected.Item as CustomList;
                        await AddMovieToList(list);
                    });
                }
                return _addToListCommand;
            }
        }

        public Command CreateListCommand
        {
            get
            {
                return this._createListCommand ?? (this._createListCommand = new  Command(
                 async () =>
                  {
                      await CreateList();
                  },
                  () =>
                  {
                      return true;
                  }));
            }
        }

        private async Task LoadList()
        {
            try
            {
                MovieList = await _listRepo.GetAllAsync();
                HasData = MovieList.Count() > 0 ? false : true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError("Getting In Theater movies", ex);
            }
        }

        private async Task AddMovieToList(CustomList customList)
        {
            var result = true;
            try
            {
                var movie =await  _movieRepo.Get(x => x.MovieId == _selectedMovie.Id);
                if (movie == null)
                {
                    var newMovie = new MovieDX.Forms.Models.Movie
                    {
                        MovieName = _selectedMovie.OriginalTitle,
                        ToWatch = true,
                        PosterURL = _selectedMovie.PosterUrl,
                        MovieRate = _selectedMovie?.Score == null ? "N/A" : _selectedMovie?.Score.ToString(),
                        MovieDescription = _selectedMovie.Overview,
                        MovieId = _selectedMovie.Id,
                        DateAdded = DateTime.Now,
                        ListId = customList.id
                    };

                    await _movieRepo.Insert(newMovie);
                }
                else
                {
                    if (movie.ListId != 0)
                    {
                        result = false;
                        await DisplayDialog("MovieDX", "This movie is already in other list", "Ok");
                    }
                    else
                    {
                        movie.ListId = customList.id;
                        await _movieRepo.Update(movie);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError("ERROR: Adding movie to list", ex);
            }
			if (result)
			{
				await DisplayDialog("MovieDX", "Movie added to list.", "Ok");
				await GoBack();
			}
        }

        private async Task CreateList()
        {
            var list = new CustomList() {
                Name = NewList,
                DateAdded = DateTime.Today
            };

            await _listRepo.Insert(list);

            //refresh
            await LoadList();
        }
    }
}
