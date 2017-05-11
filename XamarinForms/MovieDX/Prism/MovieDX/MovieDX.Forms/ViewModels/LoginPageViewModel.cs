using Acr.UserDialogs;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieDX.Core;
using MovieDX.Core.Helpers;
using Prism.Navigation;
using MovieDX.Forms.Interfaces;
using Xamarin.Forms;
using MovieDX.Forms.DataAccess;
using MovieDX.Forms.Models;
using Prism.Services;

namespace MovieDX.Forms.ViewModels
{
	public class LoginPageViewModel : BaseViewModel
	{
		private readonly IRepository<User> _userRepo;

		private string _userName;
		public string UserName
		{
			get { return _userName; }
			set { SetProperty(ref _userName, value); }
		}

		private string _password;
		public string Password
		{
			get { return _password; }
			set { SetProperty(ref _password, value); }
		}

		//Delegates
		public DelegateCommand LoginCommand { get; set; }
		public DelegateCommand SignUpCommand { get; set; }

		public LoginPageViewModel(IPageDialogService pageDialogService, INavigationService navigationService)
			: base(pageDialogService, navigationService){
            try
            {
				var connectionService = Xamarin.Forms.DependencyService.Get<ISQLite>();
				_userRepo = new Repository<User>(connectionService);

                LoginCommand = new DelegateCommand(async () => await Login());
				SignUpCommand = new DelegateCommand(async () => await SignUp());
            }
            catch (Exception ex)
            {
				ErrorLog.LogError("Login", ex);
            }
		}

		private async Task Login()
		{
			try
			{
				if (string.IsNullOrWhiteSpace(UserName)
				   && string.IsNullOrWhiteSpace(Password))
				{
					await DisplayDialog("Required fields", "All fields are required", "Ok");
					return;
				}

				var user = await _userRepo.Get(x => x.UserName == UserName);

				if (user.Password == Password)
				{
					user.IsLoggedIn = true;
					await _userRepo.Update(user);

					await NavigateToUri(Constants.InitialUrl);
				}
				else
				{ 
					await DisplayDialog("Invalid info", "UserName or Password is invalid", "Ok");
				}
			}
			catch (Exception ex)
			{
				await DisplayDialog("Invalid info", "UserName or Password is invalid", "Ok");
				ErrorLog.LogError("Save User", ex);
			}
		}

		private async Task SignUp()
		{
			await NavigateToUri(Constants.SignUpPage);
		}
	}
}
