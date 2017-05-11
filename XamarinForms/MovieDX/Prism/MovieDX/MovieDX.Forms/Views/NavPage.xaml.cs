using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MovieDX.Forms.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NavPage : NavigationPage, INavigationPageOptions
    {
        public NavPage()
        {
            InitializeComponent();
        }

        public bool ClearNavigationStackOnNavigation
        {
            get { return true; }
        }
    }
}
