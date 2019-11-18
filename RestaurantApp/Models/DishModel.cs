using System;
using RestaurantApp.Enums;

namespace RestaurantApp.Models
{
    public class DishModel
    {
        public Guid DishId { set; get; } 
        public string Name { set; get; }
        public string Description { set; get; }
        public string ImageName { set; get; }
        public DishType DishType { set; get; }
    }
}
