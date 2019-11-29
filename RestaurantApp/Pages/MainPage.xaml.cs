using System;
using System.Collections.Generic;
using RestaurantApp.Models;
using Xamarin.Forms;

namespace RestaurantApp.Pages
{
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            InitializeComponent();

            MasterPage.listView.ItemSelected += OnItemSelected;
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is MasterPageItem item)
            {
                Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
                MasterPage.listView.SelectedItem = null;
                IsPresented = false;
            }
        }
    }
}
