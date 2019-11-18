using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RestaurantApp.Enums;
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
            database.CreateTableAsync<DishModel>().Wait();
        }


        public async Task DeleteItemByIdAsync(Guid id)
        {
            await database.Table<DishModel>().Where(i => i.DishId == id).DeleteAsync();
        }

        public async Task<List<DishModel>> GetRecordsAsync(DishType? type = null)
        {
            if (type != null)
                return await database.Table<DishModel>().Where(i => i.DishType == type).ToListAsync();

            else
                return await database.Table<DishModel>().ToListAsync();
        }

        public async Task SaveItemAsync(DishModel item)
        {
            await database.InsertAsync(item);
        }
    }
}
