using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using TestSQL.DataAccess;
using System.Collections.ObjectModel;
using TestSQL.Models;

namespace TestSQL.Pages
{
    public partial class Home : ContentPage
    {
		ToDoDatabase database;
		public ObservableCollection<ToDoItem> Items { get; set; }

        public Home()
        {
            InitializeComponent();
			database = new ToDoDatabase ();

			Items = new ObservableCollection<ToDoItem> ();

			ItemList.ItemsSource = Items;

			SaveItemButton.Clicked += async (sender, e) => 	{
				if (string.IsNullOrWhiteSpace(txtItem.Text))
					return;

				database.SaveItem(new ToDoItem{
					Name = txtItem.Text
				});

				RefreshList();
				txtItem.Text = string.Empty;
			};
			RefreshList ();
        }


		private void RefreshList()
		{
			Items.Clear ();

			var items = (from i in database.GetItems<ToDoItem> ()
				orderby i.Created descending
			             select i);

			foreach (var item in items)
				Items.Add (item);
		}
    }
}
