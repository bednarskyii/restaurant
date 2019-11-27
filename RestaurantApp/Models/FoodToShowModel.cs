using System;
using Xamarin.Forms;

namespace RestaurantApp.Models
{
    public class FoodToShowModel
    {
        public string Name { set; get; }
        public string Description { set; get; }
        public decimal? Price { set; get; }
        public int Count { set; get; }

        public ImageSource PhotoSource { get; set; }
    }
}
