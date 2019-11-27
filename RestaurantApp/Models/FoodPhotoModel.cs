using System;
using SQLite;

namespace RestaurantApp.Models
{
    public class FoodPhotoModel
    {
        public int PhotoId { get; set; }

        public byte[] PhotoByteData { get; set; }

    }
}
