using System;
using System.Collections.Generic;
using RestaurantApp.ViewModels;
using Xamarin.Forms;

namespace RestaurantApp.Pages
{
    public partial class ManageCategoriesPage : ContentPage
    {
        public ManageCategoriesPage()
        {
            BindingContext = new ManageCategoriesViewModel();

            InitializeComponent();
        }
    }
}
