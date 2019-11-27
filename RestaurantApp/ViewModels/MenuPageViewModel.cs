using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using RestaurantApp.Database;
using RestaurantApp.Models;
using Xamarin.Forms;

namespace RestaurantApp.ViewModels
{
    public class MenuPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<FoodToShowModel> dishes;
        private ObservableCollection<CategoryModel> categories;
        private IDatabaseRepository _database;
        private CategoryModel selectedCategory;
        private FoodToShowModel selectedFood;
        private List<FoodToShowModel> itemsToOrder = new List<FoodToShowModel>();
        private ObservableCollection<FoodToShowModel> orderedFoodList;
        private decimal? totalCost = 0;
        private FoodToShowModel selectedFoodFromOrder;

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

        public FoodToShowModel SelectedFoodFromOrder
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

        public ObservableCollection<FoodToShowModel> OrderedFoodList
        {
            get => orderedFoodList;
            set
            {
                orderedFoodList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OrderedFoodList)));
            }
        }

        public FoodToShowModel SelectedFood
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

        public ObservableCollection<FoodToShowModel> Dishes
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
            List<FoodModel> foosList = await _database.GetRecordsAsync(category.CategoryId);
            List<FoodToShowModel> showFoodList = new List<FoodToShowModel>();

            foreach (FoodModel food in foosList)
            {
                ImageSource imageSource = null;

                List<FoodPhotoModel> foodPhoto = await _database.GetPhoto(food.PhotoId);
                if (foodPhoto.Count > 0)
                {
                    var foodImageByteArray = foodPhoto[0].PhotoByteData;
                    imageSource = ImageSource.FromStream(() => new MemoryStream(foodImageByteArray));
                }

                showFoodList.Add(new FoodToShowModel
                {   Count = food.Count,
                    Description = food.Description,
                    Name = food.Name,
                    Price = food.Price,
                    PhotoSource = imageSource});
            }

            Dishes = new ObservableCollection<FoodToShowModel>(showFoodList);
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
            OrderedFoodList = new ObservableCollection<FoodToShowModel>(itemsToOrder);
        }

        private void AddSelectedFoodToOrder()
        {
            if (!itemsToOrder.Contains(selectedFood))
            {
                itemsToOrder.Add(selectedFood);
                TotalCost += selectedFood.Price;
                SelectedFoodFromOrder = selectedFood;
            }

            InitializeOrder();
        }

        private void OnPlusItemClicked()
        {
            if (SelectedFoodFromOrder != null)
            {
                SelectedFoodFromOrder.Count += 1;
                InitializeOrder();
                TotalCost += SelectedFoodFromOrder.Price;
            }
        }

        private void OnMinusItemClicked()
        {
            if (SelectedFoodFromOrder != null && SelectedFoodFromOrder.Count >= 1)
            {
                SelectedFoodFromOrder.Count -= 1;
                TotalCost -= SelectedFoodFromOrder.Price;

                if (SelectedFoodFromOrder.Count == 0)
                {
                    SelectedFoodFromOrder.Count += 1;
                    itemsToOrder.Remove(SelectedFoodFromOrder);
                    SelectedFoodFromOrder = null;
                }
                InitializeOrder();
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
