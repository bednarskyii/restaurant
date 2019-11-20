using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Acr.UserDialogs;
using RestaurantApp.Database;
using RestaurantApp.Models;
using Xamarin.Forms;

namespace RestaurantApp.ViewModels
{
    public class ManageCategoriesViewModel : INotifyPropertyChanged
    {
        private IDatabaseRepository database;
        private ObservableCollection<CategoryModel> categoriesList;
        private string newCategoryName;
        private string enterCategoryName;
        private List<string> categoriesNames;

        public event PropertyChangedEventHandler PropertyChanged;
        public Command AddCategory { get; set; }
        public Command DeleteCategory { get; set; }
        public CategoryModel SelectedCategory { get; set; }
        public bool IsCategoriesNoteVissible;

        public string NewCategoryName
        {
            get => newCategoryName;

            set
            {
                newCategoryName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewCategoryName)));
            }
        }

        public string EnterCategoryName
        {
            get => enterCategoryName;

            set
            {
                enterCategoryName = value;
                if (categoriesNames.Contains(value))
                    IsCategoriesNoteVissible = true;
                else
                    IsCategoriesNoteVissible = false;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EnterCategoryName)));
            }
        }


        public ObservableCollection<CategoryModel> CategoriesList
        {
            get => categoriesList;

            set
            {
                categoriesList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CategoriesList))); 
            }
        }

        public ManageCategoriesViewModel()
        {
            database = new DatabaseRepository();
            AddCategory = new Command(() => OnAddCategoryClicked());
            DeleteCategory = new Command(() => OnDeleteCategoryClicked());

            InitializeCategoriesList();
        }

        private async Task InitializeCategoriesList()
        {
            CategoriesList = new ObservableCollection<CategoryModel>(await database.GetCategory());

            categoriesNames = new List<string>();
            foreach (CategoryModel category in CategoriesList)
            {
                categoriesNames.Add(category.CategoryName);
            }
        }

        private async Task OnAddCategoryClicked()
        {
            await database.AddCategory(newCategoryName);
            await InitializeCategoriesList();
            NewCategoryName = null;
        }

        private async Task OnDeleteCategoryClicked()
        {
            if (SelectedCategory != null)
            {
                ConfirmConfig config = new ConfirmConfig()
                {
                    Message = $"Delete the {SelectedCategory.CategoryName} category?",
                    OkText = "Delete",
                    CancelText = "Cancel"
                };

                var res = await UserDialogs.Instance.ConfirmAsync(config);

                if (res)
                {
                    await database.DeleteCategory(SelectedCategory.CategoryId);
                    CategoriesList.Remove(SelectedCategory);
                    categoriesNames.Remove(SelectedCategory.CategoryName);
                }
            }
            else
            {
                UserDialogs.Instance.Alert("Select an category, please");
            }
        }

    }
}
