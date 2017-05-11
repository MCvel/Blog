using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Services;
using Prism.Navigation;
using System.Threading.Tasks;
using MovieDX.Forms.Interfaces;
using MovieDX.Forms.Models;
using MovieDX.Core.Helpers;
using MovieDX.Forms.DataAccess;

namespace MovieDX.Forms.ViewModels
{
	public class SettingsPageViewModel : BaseViewModel
	{
        private readonly IRepository<User> _userRepo;
        public DelegateCommand LogoutCommand { get; set; }

        public SettingsPageViewModel(IPageDialogService pageDialogService, INavigationService navigationService)
			: base(pageDialogService, navigationService)
		{
            var connectionService = Xamarin.Forms.DependencyService.Get<ISQLite>();
            _userRepo = new Repository<User>(connectionService);

            LogoutCommand = new DelegateCommand(async () => await LognOut());
        }

        async Task LognOut()
        {
            try
            {
                var user = await _userRepo.Get(x => x.IsLoggedIn == true);
                user.IsLoggedIn = false;

                await _userRepo.Update(user);

                await NavigateToUri(Constants.InitialUrlLoggedOut);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError("Save User", ex);
            }
        }
    }
}
