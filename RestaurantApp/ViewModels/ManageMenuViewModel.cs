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
    public class ManageMenuViewModel : INotifyPropertyChanged
    {
        private IDatabaseRepository database;
        private ObservableCollection<CategoryModel> categoriesList;
        private ObservableCollection<CategoryModel> categoriesListToShow;
        private string enterCategoryName;
        private string showHideText = "▼";
        private bool isListVisible;
        private CategoryModel selectedCategory;
        private ObservableCollection<CategoryGroup> groupedList;
        private string newFoodName;
        private decimal? newPrice;
        private string newFoodDescription;
        private bool isEditingButonVissible;

        public FoodModel SelectedFood { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public Command ShowHideCommand { get; set; }
        public Command AddNewFood { get; set; }
        public Command EditFood { get; set; }
        public Command DeleteFood { get; set; }
        public Command CancelEdit { get; set; }
        public Command SaveEdit { get; set; }

        public ManageMenuViewModel()
        {
            database = new DatabaseRepository();
            ShowHideCommand = new Command(() => OnShowHideClicked());
            AddNewFood = new Command(() => OnAddNewFoodClicked());
            EditFood = new Command(() => OnEditFoodClicked());
            DeleteFood = new Command(() => OnDeleteFoodClicked());
            CancelEdit = new Command(() => OnCancelEditClicked());
            SaveEdit = new Command(() => OnSaveEditClicked());

            InitializeCategoriesList();
        }

        public decimal? NewPrice
        {
            get => newPrice;

            set
            {
                newPrice = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewPrice)));
            }
        }

        public string NewFoodName
        {
            get => newFoodName;

            set
            {
                newFoodName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewFoodName)));
            }
        }

        public string NewFoodDescription
        {
            get => newFoodDescription;

            set
            {
                newFoodDescription = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewFoodDescription)));
            }
        }


        public ObservableCollection<CategoryGroup> GroupedList
        {
            get => groupedList;

            set
            {
                groupedList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GroupedList)));
            }
        }

        public CategoryModel SelectedCategory
        {
            get => selectedCategory;

            set
            {
                if (value != null)
                {
                    selectedCategory = value;
                    IsListVisible = false;
                    EnterCategoryName = value.CategoryName;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCategory)));
            }
        }

        public bool IsListVisible
        {
            get => isListVisible;

            set
            {
                isListVisible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsListVisible)));
            }
        }

        public bool IsEditingButonVissible
        {
            get => isEditingButonVissible;

            set
            {
                isEditingButonVissible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEditingButonVissible)));
            }
        }


        public string ShowHideText
        {
            get => showHideText;

            set
            {
                showHideText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowHideText)));
            }
        }

        public string EnterCategoryName
        {
            get => enterCategoryName;

            set
            {
                enterCategoryName = value;
                CategoryNameListConsist();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EnterCategoryName)));
            }
        }

        public ObservableCollection<CategoryModel> CategoriesList
        {
            get => categoriesList;

            set
            {
                categoriesList = value;
                if (value != null)
                {
                    CategoriesListToShow = new ObservableCollection<CategoryModel>(value);
                    InitializeGroups();
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CategoriesList)));
            }
        }

        public ObservableCollection<CategoryModel> CategoriesListToShow
        {
            get => categoriesListToShow;

            set
            {
                categoriesListToShow = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CategoriesListToShow)));
            }
        }


        public async Task InitializeCategoriesList()
        {
            CategoriesList = new ObservableCollection<CategoryModel>(await database.GetCategory());
            GroupedList = new ObservableCollection<CategoryGroup>();
        }

        private void CategoryNameListConsist()
        {
            CategoriesListToShow = new ObservableCollection<CategoryModel>(CategoriesList);
            if (!string.IsNullOrEmpty(EnterCategoryName))
            {
                foreach (CategoryModel category in CategoriesList)
                {
                    if (!category.CategoryName.Contains(EnterCategoryName) && CategoriesListToShow.Contains(category))
                        CategoriesListToShow.Remove(category);
                }

                if (CategoriesListToShow.Count < 1)
                {
                    IsListVisible = false;
                    ShowHideText = "▼";
                }
                else if (CategoriesListToShow.Count == 1 && categoriesListToShow[0].CategoryName == EnterCategoryName)
                {
                    IsListVisible = false;
                    ShowHideText = "▼";
                }
                else
                {
                    IsListVisible = true;
                }
            }
        }

        private async Task OnShowHideClicked()
        {
            if (ShowHideText.Contains("▼"))
            {
                ShowHideText = "▲";
                IsListVisible = true;
            }
            else
            {
                IsListVisible = false;
                ShowHideText = "▼";
            }
        }

        private async Task OnAddNewFoodClicked()
        {
            await database.SaveItemAsync(new FoodModel { DishId = Guid.NewGuid(), Name = NewFoodName, Description = NewFoodDescription, DishTypeId = SelectedCategory.CategoryId, Price = NewPrice });
            await InitializeGroups();

            NewFoodName = null;
            NewFoodDescription = null;
            SelectedCategory = null;
            NewPrice = null;
        }

        public async Task InitializeGroups()
        {
            List<CategoryGroup> groupList = new List<CategoryGroup>();

            foreach (CategoryModel category in CategoriesList)
            {
                List<FoodModel> dishes = await database.GetRecordsAsync(category.CategoryId);

                groupList.Add(new CategoryGroup(category.CategoryName, new List<FoodModel>(dishes)));
            }

            GroupedList = new ObservableCollection<CategoryGroup>(groupList);
        }

        private async Task OnDeleteFoodClicked()
        {
            await database.DeleteItemByIdAsync(SelectedFood.DishId);
            SelectedFood = null;
            await InitializeGroups();
        }

        private async Task OnEditFoodClicked()
        {
            IsEditingButonVissible = true;

            NewFoodDescription = SelectedFood.Description;
            NewFoodName = SelectedFood.Name;
            List<CategoryModel> category = await database.GetCategory(SelectedFood.DishTypeId);
            SelectedCategory = category[0];
            NewPrice = SelectedFood.Price;
        }

        private void OnCancelEditClicked()
        {
            IsEditingButonVissible = false;

            NewFoodDescription = null;
            NewFoodName = null;
            SelectedCategory = null;
            EnterCategoryName = null;
            NewPrice = null;
        }

        private async Task OnSaveEditClicked()
        {
            await database.DeleteItemByIdAsync(SelectedFood.DishId);
            await OnAddNewFoodClicked();
            EnterCategoryName = null;
            IsEditingButonVissible = false;
        }
    }
}
