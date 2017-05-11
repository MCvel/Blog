using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Moq;
using Prism.Services;
using Prism.Navigation;

namespace MovieDX.Tests
{
	[TestFixture()]
	public class Test
	{
		[SetUp]
		public void SetUp()
		{
			Xamarin.Forms.Mocks.MockForms.Init();
		}

		[Test()]
		public void TrueTest()
		{
			int expected = 1;
			int actual = 1;

			Assert.AreEqual(expected, actual);
		}

		[Test()]
		public async Task SearchMovieTest()
		{
			var movieServiceMock = new MovieDX.Core.TMDBMovieService();

			var pageDialogMock = new Mock<IPageDialogService>();
			var navigationMock = new Mock<INavigationService>();

			MovieDX.Forms.ViewModels.SearchMoviePageViewModel searchVM;

			searchVM = new MovieDX.Forms.ViewModels.SearchMoviePageViewModel(pageDialogMock.Object, navigationMock.Object, movieServiceMock);
			searchVM.SearchField = "memento"; //search criteria

			await searchVM.SearchMovie();
			await Task.Delay(1000);
			Assert.IsNotNull(searchVM.SearchList);
		}
	}
}
