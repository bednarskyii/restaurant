using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using RestaurantApp.Database;
using RestaurantApp.Models;
using Xamarin.Forms;

namespace RestaurantApp.ViewModels
{
    public class MenuPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<FoodModel> dishes;
        private ObservableCollection<CategoryModel> categories;
        private IDatabaseRepository _database;
        private CategoryModel selectedCategory;
        private FoodModel selectedFood;
        private List<FoodModel> itemsToOrder = new List<FoodModel>();
        private ObservableCollection<FoodModel> orderedFoodList;
        private decimal? totalCost = 0;
        private FoodModel selectedFoodFromOrder;

        public Command PlusItemInOrder { get; set; }
        public Command MinusItemInOrder { get; set; }
        public Command DeleteFoodFromOrder { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public MenuPageViewModel()
        {
            _database = new DatabaseRepository();

            PlusItemInOrder = new Command(() => OnPlusItemClicked());
            MinusItemInOrder = new Command(() => OnMinusItemClicked());
            DeleteFoodFromOrder = new Command(() => OnDeleteFromOrderClicked());

            InitializeCategories();
            InitializeDishesList(null);
        }

        public FoodModel SelectedFoodFromOrder
        {
            get => selectedFoodFromOrder;

            set
            {
                selectedFoodFromOrder = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedFoodFromOrder)));
            }
        }

        public decimal? TotalCost
        {
            get => totalCost;

            set
            {
                totalCost = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalCost)));
            }
        }

        public ObservableCollection<FoodModel> OrderedFoodList
        {
            get => orderedFoodList;
            set
            {
                orderedFoodList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OrderedFoodList)));
            }
        }

        public FoodModel SelectedFood
        {
            get => selectedFood;

            set
            {
                selectedFood = value;
                AddSelectedFoodToOrder();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedFood)));
            }
        }

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

        public async Task InitializeDishesList(CategoryModel category)
        {
            Dishes = new ObservableCollection<FoodModel>(await _database.GetRecordsAsync(category.CategoryId));
        }

        public async Task InitializeCategories()
        {
            List<CategoryModel> categories = await _database.GetCategory();
            List<CategoryModel> finalcategories = await _database.GetCategory();

            //TODO code doesn't work 
            foreach (CategoryModel category in categories)
            {
                List<FoodModel> foods = await _database.GetRecordsAsync(category.CategoryId);
                if (foods.Count < 1)
                {
                    try
                    {
                        finalcategories.Remove(category);
                    }
                    catch (Exception e)
                    {
                        var t = e.Message;
                    }
                }
            }

            Categories = new ObservableCollection<CategoryModel>(finalcategories);
        }

        private void InitializeOrder()
        {
            OrderedFoodList = new ObservableCollection<FoodModel>(itemsToOrder);
        }

        private void AddSelectedFoodToOrder()
        {
            if (!itemsToOrder.Contains(selectedFood))
            {
                itemsToOrder.Add(selectedFood);
                TotalCost += selectedFood.Price;
            }

            InitializeOrder();
        }

        private void OnPlusItemClicked()
        {
            if (SelectedFoodFromOrder != null)
            {
                itemsToOrder.Remove(SelectedFoodFromOrder);
                SelectedFoodFromOrder.Count += 1;
                itemsToOrder.Add(SelectedFoodFromOrder);
                InitializeOrder();
                TotalCost += SelectedFoodFromOrder.Price;
            }
        }

        private void OnMinusItemClicked()
        {
            if (SelectedFoodFromOrder != null && SelectedFoodFromOrder.Count > 1)
            {
                itemsToOrder.Remove(SelectedFoodFromOrder);
                SelectedFoodFromOrder.Count -= 1;
                itemsToOrder.Add(SelectedFoodFromOrder);
                InitializeOrder();
                TotalCost -= SelectedFoodFromOrder.Price;
            }
        }

        private void OnDeleteFromOrderClicked()
        {
            if (SelectedFoodFromOrder != null)
            {
                itemsToOrder.Remove(SelectedFoodFromOrder);
                TotalCost -= (SelectedFoodFromOrder.Count * SelectedFoodFromOrder.Price);
                SelectedFoodFromOrder = null;
                InitializeOrder();
            }
        }
    }
}
