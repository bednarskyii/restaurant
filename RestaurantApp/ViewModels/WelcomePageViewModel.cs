using System;
using System.Threading.Tasks;
using RestaurantApp.Pages;
using Xamarin.Forms;

namespace RestaurantApp.ViewModels
{
    public class WelcomePageViewModel
    {
        private INavigation Navigation { get; set; }
        public Command Start { get; set; } 

        public WelcomePageViewModel(INavigation navigation)
        {
            Navigation = navigation;

            Start = new Command(() => OnStartClicked());
        }

        private async Task OnStartClicked()
        {
            await Navigation.PushModalAsync(new MainPage());
        }
    }
}
