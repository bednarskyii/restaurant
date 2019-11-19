using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using RestaurantApp.Database;
using RestaurantApp.Enums;
using RestaurantApp.Models;

namespace RestaurantApp.ViewModels
{
    public class MenuPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<DishModel> _dishes;
        private IDatabaseRepository _database;
        private DishType _selectedDishType;

        private ObservableCollection<DishModel> selectedDishes;
        public ObservableCollection<DishModel> SelectedDishes
        {
            get
            {
                return selectedDishes;
            }
            set
            {
                selectedDishes = value;
            }
        }


        public List<string> DishTypes { get; set; }

        public ObservableCollection<DishModel> Dishes
        {
            get => _dishes;

            set
            {
                _dishes = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Dishes))); 
            }
        }

        public DishType SelectedDish
        {
            get => _selectedDishType;

            set
            {
                _selectedDishType = value;
                //InitializeDishesList(_selectedDishType);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MenuPageViewModel()
        {
            _database = new DatabaseRepository();
            var listTypes = Enum.GetNames(typeof(DishType)).ToList();
            DishTypes = new List<string>(listTypes);

            InitializeDishesList(DishType.Pizza);
        }

        public async Task InitializeDishesList(DishType type)
        {
            Dishes = new ObservableCollection<DishModel>(await _database.GetRecordsAsync());

            SelectedDishes = new ObservableCollection<DishModel>()
            {
                Dishes[1], Dishes[3]
            };
        }
    }
}
