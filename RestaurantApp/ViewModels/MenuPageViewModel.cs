using System;
using System.Collections.Generic;

namespace RestaurantApp.ViewModels
{
    public class MenuPageViewModel
    {
        public string[] Dishes { get; set; } = new string[]
        {
            "Baboon",
            "Capuchin Monkey",
            "Blue Monkey",
            "Squirrel Monkey",
            "Golden Lion Tamarin",
            "Howler Monkey",
            "Japanese Macaque"
        };

        public MenuPageViewModel()
        {
        }
    }
}
