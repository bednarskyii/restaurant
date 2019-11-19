using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using RestaurantApp.Database;
using RestaurantApp.Models;

namespace RestaurantApp.ViewModels
{
    public class ManageCategoriesViewModel : INotifyPropertyChanged
    {
        private IDatabaseRepository database;
        private ObservableCollection<CategoryModel> categoriesList;

        public event PropertyChangedEventHandler PropertyChanged;

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

            InitializeCategories();
        }

        private async Task InitializeCategories()
        {
            CategoriesList = new ObservableCollection<CategoryModel>(await database.GetCategory());
        }
    }
}
