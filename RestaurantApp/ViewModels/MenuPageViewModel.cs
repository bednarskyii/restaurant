using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using RestaurantApp.Database;
using RestaurantApp.Models;

namespace RestaurantApp.ViewModels
{
    public class MenuPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<FoodModel> dishes;
        private ObservableCollection<CategoryModel> categories;
        private IDatabaseRepository _database;
        private CategoryModel selectedCategory;

        public CategoryModel SelectedCategory
        {
            get => selectedCategory;

            set
            {
                selectedCategory = value;
                InitializeDishesList(selectedCategory);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCategory)));
            }
        }

        public ObservableCollection<FoodModel> Dishes
        {
            get => dishes;

            set
            {
                dishes = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Dishes))); 
            }
        }

        public ObservableCollection<CategoryModel> Categories
        {
            get => categories;

            set
            {
                categories = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Categories)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MenuPageViewModel()
        {
            _database = new DatabaseRepository();

            InitializeCategories();
            InitializeDishesList(null);
        }

        public async Task InitializeDishesList(CategoryModel category)
        {
            Dishes = new ObservableCollection<FoodModel>(await _database.GetRecordsAsync(category.CategoryId));
        }

        public async Task InitializeCategories()
        {
            Categories = new ObservableCollection<CategoryModel>(await _database.GetCategory());
        }
    }
}
