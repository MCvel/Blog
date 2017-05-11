using MovieDX.Core.Interfaces;
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
    public class MovieListDetailPageViewModel : BaseViewModel
    {
        private readonly IMovieService _movieService;
        private Movie _movie { get; set; }

        public MovieListDetailPageViewModel(IPageDialogService pageDialogService, INavigationService navigationService, IMovieService movieService)
            : base(pageDialogService, navigationService)
        {
            _movieService = movieService;
            Task.Run(GetMovieInfo).ConfigureAwait(true);
        }

        private async Task GetMovieInfo()
        {
            var movie = await _movieService.DetailedMovieFromId(_movie.MovieId);
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            _movie = (Movie)parameters["movie"];
        }

        public override void OnNavigatedFrom(NavigationParameters parameters)
        {
            _movie = (Movie)parameters["movie"];
        }
    }
}