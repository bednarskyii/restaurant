using System;
using System.Collections.Generic;

namespace RestaurantApp.Models
{
    public class CategoryGroup : List<FoodModel>
    {
        public string Name { get; private set; }

        public CategoryGroup(string name, List<FoodModel> animals) : base(animals)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
