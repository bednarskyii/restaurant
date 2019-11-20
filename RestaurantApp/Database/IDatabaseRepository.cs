using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestaurantApp.Enums;
using RestaurantApp.Models;

namespace RestaurantApp.Database
{
    public interface IDatabaseRepository
    {
        Task<List<DishModel>> GetRecordsAsync(DishType? type = null);
        Task SaveItemAsync(DishModel item);
        Task DeleteItemByIdAsync(Guid id);

        Task AddCategory(string name);
        Task<List<CategoryModel>> GetCategory(Guid? id = null);
        Task DeleteCategory(Guid id);
    }
}
