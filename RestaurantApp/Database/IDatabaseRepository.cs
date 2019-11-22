using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestaurantApp.Enums;
using RestaurantApp.Models;

namespace RestaurantApp.Database
{
    public interface IDatabaseRepository
    {
        Task<List<FoodModel>> GetRecordsAsync(CategoryModel type = null);
        Task SaveItemAsync(FoodModel item);
        Task DeleteItemByIdAsync(Guid id);

        Task AddCategory(string name);
        Task<List<CategoryModel>> GetCategory(Guid? id = null);
        Task DeleteCategory(Guid id);
    }
}
