using RestaurantApp.ViewModels;
using Xamarin.Forms;

namespace RestaurantApp.Pages
{
    public partial class WelcomePage : ContentPage
    {
        public WelcomePage()
        {
            InitializeComponent();
            BindingContext = new WelcomePageViewModel(Navigation);
        }
    }
}
