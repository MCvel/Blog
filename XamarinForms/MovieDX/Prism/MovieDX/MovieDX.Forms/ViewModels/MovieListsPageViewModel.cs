using MovieDX.Core.Helpers;
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
    public class MovieListsPageViewModel : BaseViewModel
    {
        public bool HasData { get; set; } = false;
        private readonly IRepository<CustomList> _listRepo;

        private DelegateCommand<ItemTappedEventArgs> _selectListCommand;

        private List<CustomList> _movieList;
        public List<CustomList> MovieList
        {
            get { return _movieList; }
            set { SetProperty(ref _movieList, value); }
        }

        public MovieListsPageViewModel(IPageDialogService pageDialogService, INavigationService navigationService)
            : base(pageDialogService, navigationService)
        {
            var connectionService = Xamarin.Forms.DependencyService.Get<ISQLite>();
            _listRepo = new Repository<CustomList>(connectionService);

            Task.Run(LoadList).ConfigureAwait(true);
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
                ErrorLog.LogError("ERROR: Getting Lists", ex);
            }
        }

        public DelegateCommand<ItemTappedEventArgs> SelectListCommand
        {
            get
            {
                if (_selectListCommand == null)
                {
                    _selectListCommand = new DelegateCommand<ItemTappedEventArgs>(async selected =>
                    {
                        var list = selected.Item as CustomList;
                        await SelectList(list);
                    });
                }
                return _selectListCommand;
            }
        }

        private async Task SelectList(CustomList customList)
        {
            try
            {
                var param = new NavigationParameters();
                param.Add("list", customList);

				await NavigateToUriWithModalOption(Constants.MovieListInfoPage, param, false);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError("ERROR: selecting list", ex);
            }
        }
    }
}
