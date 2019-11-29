using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Acr.UserDialogs;
using RestaurantApp.Database;
using RestaurantApp.DependencyServices;
using RestaurantApp.Models;
using Xamarin.Forms;

namespace RestaurantApp.ViewModels
{
    public class ManageMenuViewModel : INotifyPropertyChanged
    {
        private IDatabaseRepository database;
        private IPhotoPickerService photoPicker;
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
        private byte[] foodImageByteArray;
        private ImageSource newImageSource;

        public FoodModel SelectedFood { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public Command ShowHideCommand { get; set; }
        public Command AddNewFood { get; set; }
        public Command EditFood { get; set; }
        public Command DeleteFood { get; set; }
        public Command CancelEdit { get; set; }
        public Command SaveEdit { get; set; }
        public Command ChooseImage { get; set; }

        public ManageMenuViewModel()
        {
            photoPicker = DependencyService.Get<IPhotoPickerService>();
            database = new DatabaseRepository();
            ShowHideCommand = new Command(() => OnShowHideClicked());
            AddNewFood = new Command(() => OnAddNewFoodClicked());
            EditFood = new Command(() => OnEditFoodClicked());
            DeleteFood = new Command(() => OnDeleteFoodClicked());
            CancelEdit = new Command(() => OnCancelEditClicked());
            SaveEdit = new Command(() => OnSaveEditClicked());
            ChooseImage = new Command(() => OnChooseImageClicked());

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

        public ImageSource NewImageSource
        {
            get => newImageSource;

            set
            {
                newImageSource = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewImageSource)));
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

        private void OnShowHideClicked()
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
            //not a best practice with random
            Random rand = new Random();
            int randomId = rand.Next(0, 1000);
            FoodPhotoModel newImage = new FoodPhotoModel { PhotoId = randomId, PhotoByteData = foodImageByteArray };
            await database.AddPhoto(newImage);

            await database.SaveItemAsync(new FoodModel { DishId = Guid.NewGuid(), PhotoId = randomId, Name = NewFoodName, Description = NewFoodDescription, DishTypeId = SelectedCategory.CategoryId, Price = NewPrice });
            await InitializeGroups();

            NewFoodName = null;
            NewFoodDescription = null;
            SelectedCategory = null;
            NewPrice = null;
            NewImageSource = null;
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
            if (SelectedFood != null)
            {
                ConfirmConfig config = new ConfirmConfig()
                {
                    Message = $"Delete the {SelectedFood.Name}?",
                    OkText = "Delete",
                    CancelText = "Cancel"
                };

                var res = await UserDialogs.Instance.ConfirmAsync(config);

                if (res)
                {
                    await database.DeleteItemByIdAsync(SelectedFood.DishId);
                    SelectedFood = null;
                    await InitializeGroups();
                }
            }
            else
            {
                UserDialogs.Instance.Alert("Please, select a food");
            }
        }

        private async Task OnEditFoodClicked()
        {
            if(SelectedFood != null)
            {
                IsEditingButonVissible = true;

                NewFoodDescription = SelectedFood.Description;
                NewFoodName = SelectedFood.Name;
                List<CategoryModel> category = await database.GetCategory(SelectedFood.DishTypeId);
                SelectedCategory = category[0];
                NewPrice = SelectedFood.Price;

                List<FoodPhotoModel> foodPhoto = await database.GetPhoto(SelectedFood.PhotoId);
                if (foodPhoto != null)
                {
                    foodImageByteArray = foodPhoto[0].PhotoByteData;
                    NewImageSource = ImageSource.FromStream(() => new MemoryStream(foodImageByteArray));
                }
            }
            else
            {
                UserDialogs.Instance.Alert("Please, select a food");
            }
        }

        private void OnCancelEditClicked()
        {
            IsEditingButonVissible = false;

            NewFoodDescription = null;
            NewFoodName = null;
            SelectedCategory = null;
            EnterCategoryName = null;
            NewPrice = null;
            NewImageSource = null;
        }

        private async Task OnSaveEditClicked()
        {
            await database.DeleteItemByIdAsync(SelectedFood.DishId);
            await database.DeletePhoto(SelectedFood.PhotoId);
            await OnAddNewFoodClicked();
            EnterCategoryName = null;
            IsEditingButonVissible = false;
        }

        private async Task OnChooseImageClicked()
        {
            var image = await photoPicker.GetImageStreamAsync();

            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = image.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                foodImageByteArray = ms.ToArray();
            }

            NewImageSource = ImageSource.FromStream(() => new MemoryStream(foodImageByteArray));
        }
    }
}
