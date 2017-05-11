using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Acr.UserDialogs;
using Prism;
using Prism.Services;
using Prism.Navigation;

namespace MovieDX.Forms.ViewModels
{
	public class HomePageViewModel : BaseViewModel
    {
		
		public HomePageViewModel(IPageDialogService pageDialogService, INavigationService navigationService)
			: base(pageDialogService, navigationService)
		{
			
        }
	}
}
