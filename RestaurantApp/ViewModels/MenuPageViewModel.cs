using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Acr.UserDialogs;
using RestaurantApp.Database;
using RestaurantApp.Models;
using Xamarin.Forms;

namespace RestaurantApp.ViewModels
{
    public class MenuPageViewModel : INotifyPropertyChanged
    {
        private INavigation _navigation;
        private IDatabaseRepository _database;
        private ObservableCollection<FoodToShowModel> dishes;
        private ObservableCollection<CategoryModel> categories;
        private CategoryModel selectedCategory;
        private FoodToShowModel selectedFood;
        private List<FoodToShowModel> itemsToOrder = new List<FoodToShowModel>();
        private ObservableCollection<FoodToShowModel> orderedFoodList;
        private FoodToShowModel selectedFoodFromOrder;
        private bool isGuestPopUpVissible;
        private bool isOrderListEmpty;
        private bool isCashierMode;
        private bool isCahierPopUpVissible;
        private bool isEndOrderPopupVisible;
        private decimal rest;
        private decimal? totalCost = 0;
        private string recievedMoney;

        public Command ContinueOrdering { get; set; }
        public Command ClearOrderList { get; set; }
        public Command AcceptOrder { get; set; }
        public Command CancelOrder { get; set; }
        public Command PlusItemInOrder { get; set; }
        public Command MakeOrder { get; set; }
        public Command MinusItemInOrder { get; set; }
        public Command DeleteFoodFromOrder { get; set; }
        public Command CloseMenu { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public MenuPageViewModel(INavigation navigation)
        {
            _navigation = navigation;
            _database = new DatabaseRepository();

            ClearOrderList = new Command(() => OnClearOrderListClicked());
            AcceptOrder = new Command(() => OnAcceptOrderClicked());
            CancelOrder = new Command(() => OnCancelOrderClicked());
            MakeOrder = new Command(() => OnMakeOrderClicked());
            PlusItemInOrder = new Command(() => OnPlusItemClicked());
            MinusItemInOrder = new Command(() => OnMinusItemClicked());
            DeleteFoodFromOrder = new Command(() => OnDeleteFromOrderClicked());
            CloseMenu = new Command(() => OnCloseMenuClicked());
            ContinueOrdering = new Command(() => OnContinueOrderingClicked());

            OrderedFoodList = new ObservableCollection<FoodToShowModel>();

            InitializeCategories();
            InitializeDishesList(null);
        }

        public string RecievedMoney
        {
            get => recievedMoney;

            set
            {
                recievedMoney = value;
                if(recievedMoney != null)
                    try
                    {
                        Rest = Convert.ToDecimal(recievedMoney) - (decimal)totalCost;
                    }
                    catch (Exception e)
                    {
                        //in cases if value is not a number
                        RecievedMoney = null;
                        Rest = 0;
                    }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RecievedMoney)));
            }
        }

        public decimal Rest
        {
            get => rest;

            set
            {
                rest = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Rest)));
            }
        }

        public bool IsCashierMode
        {
            get => isCashierMode;

            set
            {
                isCashierMode = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCashierMode)));
            }
        }

        public bool IsEndOrderPopupVisible
        {
            get => isEndOrderPopupVisible;

            set
            {
                isEndOrderPopupVisible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEndOrderPopupVisible)));
            }
        }

        public bool IsOrderListEmpty
        {
            get => isOrderListEmpty;

            set
            {
                isOrderListEmpty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOrderListEmpty)));
            }
        }

        public bool IsGuestPopUpVissible
        {
            get => isGuestPopUpVissible;

            set
            {
                isGuestPopUpVissible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsGuestPopUpVissible)));
            }
        }

        public bool IsCahierPopUpVissible
        {
            get => isCahierPopUpVissible;

            set
            {
                isCahierPopUpVissible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCahierPopUpVissible)));
            }
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
                if (OrderedFoodList == null || OrderedFoodList.Count < 1)
                    IsOrderListEmpty = true;
                else
                    IsOrderListEmpty = false;
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
            Categories = new ObservableCollection<CategoryModel>(await _database.GetCategory());
            SelectedCategory = Categories[0];
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
            else if (OrderedFoodList.Count < 1)
            {
                UserDialogs.Instance.Alert("Your order list is empty");
            }
            else
            {
                UserDialogs.Instance.Alert("Please, select a food from the list");
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
            else if (OrderedFoodList.Count < 1)
            {
                UserDialogs.Instance.Alert("Your order list is empty");
            }
            else
            {
                UserDialogs.Instance.Alert("Please, select a food from the list");
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
            else if(OrderedFoodList.Count < 1)
            {
                UserDialogs.Instance.Alert("Your order list is empty");
            }
            else
            {
                UserDialogs.Instance.Alert("Please, select a food from the list");
            }
        }

        private void OnMakeOrderClicked()
        {
            if(TotalCost > 0)
            {
                if (IsCashierMode)
                    IsCahierPopUpVissible = true;
                else
                    IsGuestPopUpVissible = true;
            }
            else
            {
                UserDialogs.Instance.Alert("Your order list is empty");
            }
        }

        private void OnCancelOrderClicked()
        {
            IsGuestPopUpVissible = false;
            IsCahierPopUpVissible = false;
        }

        private void OnAcceptOrderClicked()
        {
            IsGuestPopUpVissible = false;
            IsEndOrderPopupVisible = true;

            SelectedFoodFromOrder = null;
            IsCahierPopUpVissible = false;
            itemsToOrder = new List<FoodToShowModel>();
            InitializeOrder();
            TotalCost = 0;
        }

        private void OnClearOrderListClicked()
        {
            SelectedFoodFromOrder = null;
            itemsToOrder = new List<FoodToShowModel>();
            InitializeOrder();
            TotalCost = 0;
        }

        private async Task OnCloseMenuClicked()
        {
            await _navigation.PopModalAsync();
        }

        private void OnContinueOrderingClicked()
        {
            IsEndOrderPopupVisible = false;
        }
    }
}
