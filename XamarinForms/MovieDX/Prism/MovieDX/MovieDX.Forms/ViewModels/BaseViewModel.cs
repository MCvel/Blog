using System;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;

namespace MovieDX.Forms
{
	public abstract class BaseViewModel : BindableBase, INavigationAware
	{
		DelegateCommand<String> _navigateToUriCommand;
		DelegateCommand<String> _navigateToUriModalCommand;
		DelegateCommand<String> _navigateBackCommand;

		public DelegateCommand<String> NavigateToUriCommand => _navigateToUriCommand ?? (_navigateToUriCommand = new DelegateCommand<String>(async param => await NavigateToUriCommandExecute(param), CanNavigateToUriCommandExecute));
		public DelegateCommand<String> NavigateToUriModalCommand => _navigateToUriModalCommand ?? (_navigateToUriModalCommand = new DelegateCommand<String>(async param => await NavigateToUriModalCommandExecute(param), CanNavigateToUriCommandExecute));
		public DelegateCommand<String> NavigateBackCommand => _navigateBackCommand ?? (_navigateBackCommand = new DelegateCommand<String>(async param => await NavigateBackCommandExecute()));

		protected INavigationService NavigationService { get; }

		protected IPageDialogService PageDialogService { get; }

		protected BaseViewModel(IPageDialogService pageDialogService, INavigationService navigationService)
		{
			if (pageDialogService == null)
			{
				throw new ArgumentNullException(nameof(pageDialogService));
			}
			if (navigationService == null)
			{
				throw new ArgumentNullException(nameof(navigationService));
			}
			this.PageDialogService = pageDialogService;
			this.NavigationService = navigationService;
		}

		public virtual void OnNavigatedFrom(NavigationParameters parameters)
		{
		}

		public virtual void OnNavigatedTo(NavigationParameters parameters)
		{
		}

		protected async Task DisplayDialog(String title, String message, String buttonText = "OK")
		{
			try
			{
				await this.PageDialogService.DisplayAlertAsync(title, message, buttonText);
			}
			catch (Exception ex)
			{
				await HandleException(ex);
			}
		}

		protected async Task<Boolean> DisplayDialog(String title, String message, String acceptButtonText, String cancellationButtonText)
		{
			try
			{
				return await this.PageDialogService.DisplayAlertAsync(title, message, acceptButtonText, cancellationButtonText);
			}
			catch (Exception ex)
			{
				await HandleException(ex);
				return false;
			}
		}

		protected async Task GoBack()
		{
			try
			{
				await this.NavigationService.GoBackAsync();
			}
			catch (Exception ex)
			{
				await HandleException(ex);
			}
		}

		protected Task HandleException(Exception ex)
		{
			var baseException = ex.GetBaseException();
			return this.PageDialogService.DisplayAlertAsync("Error", baseException.Message, "OK");
		}

		protected async Task NavigateToUriModal(String uriText)
		{
			if (String.IsNullOrWhiteSpace(uriText))
			{
				throw new ArgumentException("Value cannot be null or white space.", nameof(uriText));
			}
			try
			{
				await this.NavigationService.NavigateAsync(uriText, null, true, true);
			}
			catch (Exception ex)
			{
				await HandleException(ex);
			}
		}

		protected async Task NavigateToUri(String uriText)
		{
			if (String.IsNullOrWhiteSpace(uriText))
			{
				throw new ArgumentException("Value cannot be null or white space.", nameof(uriText));
			}
			try
			{
				await this.NavigationService.NavigateAsync(uriText);
			}
			catch (Exception ex)
			{
				await HandleException(ex);
			}
		}

		protected async Task NavigateToUri(String uriText, Object parameter, bool IsModal = false)
		{
			if (String.IsNullOrWhiteSpace(uriText))
			{
				throw new ArgumentException("Value cannot be null or white space.", nameof(uriText));
			}
			if (parameter == null)
			{
				throw new ArgumentNullException(nameof(parameter));
			}

			try
			{
				var navigationParameters = new NavigationParameters();
				navigationParameters.Add("Key", parameter);
				await NavigateToUri(uriText, navigationParameters, IsModal);
			}
			catch (Exception ex)
			{
				await HandleException(ex);
			}
		}

		protected async Task NavigateToUriWithModalOption(String uriText, NavigationParameters navigationParameters, bool IsModal = false)
		{
			if (String.IsNullOrWhiteSpace(uriText))
			{
				throw new ArgumentException("Value cannot be null or white space.", nameof(uriText));
			}
			if (navigationParameters == null)
			{
				throw new ArgumentNullException(nameof(navigationParameters));
			}
			try
			{
				await this.NavigationService.NavigateAsync(uriText, navigationParameters, IsModal);
			}
			catch (Exception ex)
			{
				await HandleException(ex);
			}
		}

		protected async Task NavigateToUri(String uriText, NavigationParameters navigationParameters)
		{
			if (String.IsNullOrWhiteSpace(uriText))
			{
				throw new ArgumentException("Value cannot be null or white space.", nameof(uriText));
			}
			if (navigationParameters == null)
			{
				throw new ArgumentNullException(nameof(navigationParameters));
			}
			try
			{
				await this.NavigationService.NavigateAsync(uriText, navigationParameters);
			}
			catch (Exception ex)
			{
				await HandleException(ex);
			}
		}

		Boolean CanNavigateToUriCommandExecute(String uriText)
		{
			return !String.IsNullOrWhiteSpace(uriText);
		}

		async Task NavigateToUriCommandExecute(String uriText)
		{
			try
			{
				await NavigateToUri(uriText);
			}
			catch (Exception ex)
			{
				await HandleException(ex);
			}
		}

		async Task NavigateToUriModalCommandExecute(String uriText)
		{
			try
			{
				await NavigateToUriModal(uriText);
			}
			catch (Exception ex)
			{
				await HandleException(ex);
			}
		}

		async Task NavigateBackCommandExecute()
		{
			try
			{
				await this.NavigationService.GoBackAsync(null, false, false);
			}
			catch (Exception ex)
			{
				await HandleException(ex);
			}
		}

		public void OnNavigatingTo(NavigationParameters parameters)
		{
			//Do nothing
		}
	}
}
