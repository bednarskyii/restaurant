using System;
using System.Collections.Generic;
using RestaurantApp.ViewModels;
using Xamarin.Forms;

namespace RestaurantApp.Pages
{
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            BindingContext = new MenuPageViewModel();
            InitializeComponent();
        }
    }
}
