using System;
using System.Collections.Generic;
using RestaurantApp.ViewModels;
using Xamarin.Forms;

namespace RestaurantApp.Pages
{
    public partial class ManageMenuPage : ContentPage
    {
        public ManageMenuPage()
        {
            BindingContext = new ManageMenuViewModel();
            InitializeComponent();
        }
    }
}
