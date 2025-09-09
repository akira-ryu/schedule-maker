using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using subcinctus_factorem.utils;
namespace subcinctus_factorem
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        public ObservableCollection<Person> employees { get; set; }
        public ObservableCollection<DateTime> CurrentWeek { get; set; }
        private Person _selectedPerson;
        public List<string> MyOptions { get; set; } = new List<string>
    {
        "Option 1",
        "Option 2",
        "Option 3"
    };

        public string SelectedOption { get; set; }
        public ICommand SelectPersonCommand => new Command<Person>(person =>
        {
            SelectedPerson = person;
        });
        public Person SelectedPerson
        {
            get => _selectedPerson;
            set
            {
                _selectedPerson = value;
                OnPropertyChanged();
            }
        }

        public MainPage()
        {
            Utils u = new Utils();
            CurrentWeek = new ObservableCollection<DateTime>(u.GetCurrentWeekDates());
            InitializeComponent();
            LoadItems();
            BindingContext = this;

            //foreach (var item in currentWeek)
            //{
            //    Console.WriteLine(item.Date);
            //}
        }

        private void LoadItems()
        {
            employees = new ObservableCollection<Person>
            {
                new Person { Title = "Person 1", Description = "Description 1",
                    shifts = new ObservableCollection<schedule>
                    {
                        new schedule { Date = "Sep 4", Position = "CSR", start = "8.00", end = "9.00" },
                        new schedule { Date = "Sep 5", Position = "Manager", start = "10.00", end = "12.00" }
                    }
                },
                new Person { Title = "Person 2", Description = "Description 2",
                    shifts = new ObservableCollection<schedule>
                    {
                        new schedule { Date = "Sep 4", Position = "Tech", start = "9.00", end = "11.00" }
                    }
                },
                new Person { Title = "Person 3", Description = "Description 3",
                    shifts = new ObservableCollection<schedule>
                    {
                        new schedule { Date = "Sep 8", Position = "Tech y", start = "9.00", end = "11.00" }
                    }
                }
            };

            SelectedPerson = employees[0]; // Default select first person
        }

        // INotifyPropertyChanged implementation
        public new event PropertyChangedEventHandler PropertyChanged;
        protected new void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Person
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public ObservableCollection<schedule> shifts { get; set; }
    }

    public class schedule
    {
        public string Date { get; set; }
        public string Position { get; set; }
        public string start { get; set; }
        public string end { get; set; }
    }
}
