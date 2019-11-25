using System;
namespace RestaurantApp.Models
{
    public class FoodModel
    {
        public Guid DishId { set; get; }
        public string Name { set; get; }
        public string Description { set; get; }
        public string ImageName { set; get; }
        public Guid DishTypeId { set; get; }
    }
}
