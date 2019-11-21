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
        private string showHideText = "Show";
        private bool isListVisible;
        private CategoryModel selectedCategory;

        public bool IsCategoriesNoteVissible;
        public event PropertyChangedEventHandler PropertyChanged;
        public Command ShowHideCommand { get; set; }

        public CategoryModel SelectedCategory
        {
            get => selectedCategory;

            set
            {
                if(value != null)
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
                if(value != null)
                   CategoriesListToShow = new ObservableCollection<CategoryModel>(value);
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


        public ManageMenuViewModel()
        {
            database = new DatabaseRepository();
            ShowHideCommand = new Command(() => OnShowHideClicked());

            InitializeCategoriesList();
        }


        private async Task InitializeCategoriesList()
        {
            CategoriesList = new ObservableCollection<CategoryModel>(await database.GetCategory());
        }

        private void CategoryNameListConsist()
        {
            CategoriesListToShow = new ObservableCollection<CategoryModel>(CategoriesList);
            if (!string.IsNullOrEmpty(EnterCategoryName))
            {
                foreach(CategoryModel category in CategoriesList)
                {
                    if (!category.CategoryName.Contains(EnterCategoryName) && CategoriesListToShow.Contains(category))
                        CategoriesListToShow.Remove(category);
                }

                if(CategoriesListToShow.Count < 1)
                {
                    IsListVisible = false;
                    ShowHideText = "Show";
                }
                else if (CategoriesListToShow.Count == 1 && categoriesListToShow[0].CategoryName == EnterCategoryName)
                {
                    IsListVisible = false;
                    ShowHideText = "Show";
                }
                else
                {
                    IsListVisible = true;
                }
            }
        }

        private void OnShowHideClicked()
        {
            if (ShowHideText == "Show")
            {
                ShowHideText = "Hide";
                IsListVisible = true;
            }
            else
            {
                IsListVisible = false;
                ShowHideText = "Show";
            }
        }

    }
}
