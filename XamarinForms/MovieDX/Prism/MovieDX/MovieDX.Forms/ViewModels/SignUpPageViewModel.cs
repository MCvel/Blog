using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Acr.UserDialogs;
using MovieDX.Core.Helpers;
using System.Threading.Tasks;
using MovieDX.Forms.Interfaces;
using MovieDX.Forms.Models;
using Xamarin.Forms;
using MovieDX.Forms.DataAccess;
using Prism.Navigation;

namespace MovieDX.Forms.ViewModels
{
    public class SignUpPageViewModel : BindableBase
	{
		readonly INavigationService _navigationService;
		private readonly IUserDialogs _userDialogs;
		private readonly IRepository<User> _userRepo;

		private string _email;
		public string Email
		{
			get { return _email; }
			set { SetProperty(ref _email, value); }
		}

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

		public SignUpPageViewModel(IUserDialogs userDialogs, INavigationService navigationService)
		{
			try
			{
				var connectionService = DependencyService.Get<ISQLite>();
				_userRepo = new Repository<User>(connectionService);

				_navigationService = navigationService;
				_userDialogs = userDialogs;

				SignUpCommand = new DelegateCommand(async () => await SignUp());
			}
			catch (Exception ex)
			{
				ErrorLog.LogError("SignUp Load", ex);
			}
		}

		//Delegates
		public DelegateCommand SignUpCommand { get; set; }

		async Task SignUp()
		{
			try
			{
				if (string.IsNullOrWhiteSpace(Email)
				   && string.IsNullOrWhiteSpace(UserName)
				   && string.IsNullOrWhiteSpace(Password))
				{
					await _userDialogs.AlertAsync("All fields are required", "Required fields", "Ok");
					return;
				}

				await _userRepo.Insert(new User
				{
					Email = Email,
					UserName = UserName,
					Password = Password,
					DateAdded = DateTime.Now
				});

				await _userDialogs.AlertAsync("User created","Info","Ok");

				await _navigationService.GoBackAsync();
			}
			catch (Exception ex)
			{
				ErrorLog.LogError("Save User", ex);
			}
		}
	}
}