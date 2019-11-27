using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RestaurantApp.Models;
using SQLite;

namespace RestaurantApp.Database
{
    public class DatabaseRepository : IDatabaseRepository
    {
        private readonly SQLiteAsyncConnection database;
        private readonly string path;

        public DatabaseRepository()
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Restaurant.db3");
            database = new SQLiteAsyncConnection(path);
            database.CreateTableAsync<CategoryModel>().Wait();
            database.CreateTableAsync<FoodModel>().Wait();
            database.CreateTableAsync<FoodPhotoModel>().Wait();
        }


        public async Task DeleteItemByIdAsync(Guid id)
        {
            await database.Table<FoodModel>().Where(i => i.DishId == id).DeleteAsync();
        }

        public async Task<List<FoodModel>> GetRecordsAsync(Guid? type = null)
        {
            if (type != null)
            {
                var d = await database.Table<FoodModel>().Where(i => i.DishTypeId == type).ToListAsync();
                return d;
            }

            else
                return await database.Table<FoodModel>().ToListAsync();
        }

        public async Task SaveItemAsync(FoodModel item)
        {
            item.Count = 1; //default Value for dishes
            if(!string.IsNullOrEmpty(item.Name))
                await database.InsertAsync(item);
        }

        public async Task AddCategory(string Name)
        {
            int count = await database.Table<CategoryModel>().Where(i => i.CategoryName == Name).CountAsync();

            if (count == 0 && !string.IsNullOrEmpty(Name))
                await database.InsertAsync(new CategoryModel { CategoryId = Guid.NewGuid(), CategoryName = Name});
        }

        public async Task<List<CategoryModel>> GetCategory(Guid? id = null)
        {
            if(id == null)
                return await database.Table<CategoryModel>().ToListAsync();
            else
            {
                return await database.Table<CategoryModel>().Where(i => i.CategoryId == id).ToListAsync();
            }
        }

        public async Task DeleteCategory(Guid id)
        {
            await database.Table<CategoryModel>().Where(i => i.CategoryId == id).DeleteAsync();
        }

        public async Task AddPhoto(FoodPhotoModel photo)
        {
            await database.InsertAsync(photo);
        }

        public async Task<List<FoodPhotoModel>> GetPhoto(int id)
        {
            return await database.Table<FoodPhotoModel>().Where(i => i.PhotoId == id).ToListAsync();
        }

        public async Task DeletePhoto(int id)
        {
            await database.Table<FoodPhotoModel>().Where(i => i.PhotoId == id).DeleteAsync();
        }
    }
}
