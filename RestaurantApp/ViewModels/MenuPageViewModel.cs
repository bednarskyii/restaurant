using System;
using System.Collections.Generic;

namespace RestaurantApp.ViewModels
{
    public class MenuPageViewModel
    {
        public List<string> DishTypes { get; set; }

        public string[] Dishes { get; set; } = new string[]
        {
            "Baboon",
            "Capuchin Monkey",
            "Blue Monkey",
            "Squirrel Monkey",
            "Golden Lion Tamarin",
            "Howler Monkey",
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
            DishTypes = new List<string>();

            DishTypes.Add("Breakfast");
            DishTypes.Add("Salats");
            DishTypes.Add("Juice");
            DishTypes.Add("Vegetarian");
            DishTypes.Add("Pizza");
            DishTypes.Add("fserwr");
            DishTypes.Add("Juirwererererce");
            DishTypes.Add("Vegeerwerwerwertarian");
            DishTypes.Add("Pizewrewrewreewza");

        }
    }
}
