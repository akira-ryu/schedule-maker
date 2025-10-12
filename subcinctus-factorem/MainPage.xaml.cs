using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using subcinctus_factorem.utils;
using subcinctus_factorem.JsonManager;
using subcinctus_factorem.Employees;
using subcinctus_factorem.Services;

namespace subcinctus_factorem
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        private HandleJson _jsonHandler;
        private PdfService _pdfService;
        private EmailService _emailService;

        public ObservableCollection<Employee> employees { get; set; }
        public ObservableCollection<DaySchedule> CurrentWeek { get; set; }
        public List<string> Timmings { get; set; }
        private Employee _selectedPerson;

        public List<string> Positions { get; set; } = new List<string>
        {
            "BAR",
            "CBS",
            "CS",
            "POS",
            "PC",

        };

        public string SelectedOption { get; set; }

        public ICommand SelectPersonCommand => new Command<Employee>(person =>
        {
            SelectedPerson = person;
        });

        public ICommand AddEmployeeCommand => new Command(async () => await AddEmployee());

        public ICommand DeleteEmployeeCommand => new Command(async () => await DeleteEmployee());

        public ICommand ExportAsPdfCommand => new Command(async () => await ExportAsPdf());

        public ICommand SendEmailCommand => new Command(async () => await SendEmail());

        public Employee SelectedPerson
        {
            get => _selectedPerson;
            set
            {
                if (_selectedPerson != value)
                {
                    if (_selectedPerson != null)
                        _selectedPerson.IsSelected = false;
                    _selectedPerson = value;
                    if (_selectedPerson != null)
                        _selectedPerson.IsSelected = true;
                    OnPropertyChanged();
                }
            }
        }

        public MainPage()
        {
            Utils u = new Utils();
            CurrentWeek = new ObservableCollection<DaySchedule>(
                u.GetCurrentWeekDates().Select(d => new DaySchedule { Date = d })
            );
            Timmings = new List<string>(u.GetTimmings());

            _pdfService = new PdfService();
            _emailService = new EmailService();

            InitializeComponent();
            LoadItems();
            BindingContext = this;
        }

        private void LoadItems()
        {
            _jsonHandler = new HandleJson();
            Utils u = new Utils();
            var weekDates = u.GetCurrentWeekDates();
            employees = new ObservableCollection<Employee>(
                _jsonHandler.Users.Select(user => new Employee
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Schedule = new ObservableCollection<DaySchedule>(
                        weekDates.Select(d => new DaySchedule { Date = d })
                    )
                })
            );
            if (employees.Count > 0)
                SelectedPerson = employees[0];
        }

        private async Task AddEmployee()
        {
            string name = await DisplayPromptAsync("Add Employee", "Enter employee name:", "OK", "Cancel");

            if (string.IsNullOrWhiteSpace(name))
                return;

            string email = await DisplayPromptAsync("Add Employee", "Enter employee email:", "OK", "Cancel", keyboard: Keyboard.Email);

            if (string.IsNullOrWhiteSpace(email))
                return;

            // Get next ID
            int nextId = _jsonHandler.Users.Any() ? _jsonHandler.Users.Max(u => u.Id) + 1 : 1;

            // Create new user
            var newUser = new User
            {
                Id = nextId,
                Name = name,
                Email = email
            };

            // Add to JSON handler and save
            _jsonHandler.AddUser(newUser);

            // Create new employee with schedule
            Utils u = new Utils();
            var weekDates = u.GetCurrentWeekDates();
            var newEmployee = new Employee
            {
                Id = newUser.Id,
                Name = newUser.Name,
                Email = newUser.Email,
                Schedule = new ObservableCollection<DaySchedule>(
                    weekDates.Select(d => new DaySchedule { Date = d })
                )
            };

            // Add to observable collection
            employees.Add(newEmployee);

            await DisplayAlert("Success", $"Employee '{name}' added successfully!", "OK");
        }

        private async Task DeleteEmployee()
        {
            if (SelectedPerson == null)
            {
                await DisplayAlert("Error", "Please select an employee to delete.", "OK");
                return;
            }

            bool confirm = await DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete '{SelectedPerson.Name}'?",
                "Yes",
                "No"
            );

            if (!confirm)
                return;

            // Remove from JSON handler and save
            _jsonHandler.DeleteUser(SelectedPerson.Id);

            // Remove from observable collection
            employees.Remove(SelectedPerson);

            // Select first employee if available
            if (employees.Count > 0)
                SelectedPerson = employees[0];
            else
                SelectedPerson = null;

            await DisplayAlert("Success", "Employee deleted successfully!", "OK");
        }

        private async Task ExportAsPdf()
        {
            if (SelectedPerson == null)
            {
                await DisplayAlert("Error", "Please select an employee to export.", "OK");
                return;
            }

            try
            {
                // Generate PDF
                string pdfPath = _pdfService.GenerateSchedulePdf(SelectedPerson);

                // Ask user what to do with the PDF
                string action = await DisplayActionSheet(
                    "PDF Generated Successfully",
                    "Cancel",
                    null,
                    "Open PDF",
                    "Share PDF"
                );

                if (action == "Open PDF")
                {
                    await Launcher.Default.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(pdfPath)
                    });
                }
                else if (action == "Share PDF")
                {
                    await Share.Default.RequestAsync(new ShareFileRequest
                    {
                        Title = $"Schedule - {SelectedPerson.Name}",
                        File = new ShareFile(pdfPath)
                    });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to generate PDF: {ex.Message}", "OK");
            }
        }

        private async Task SendEmail()
        {
            if (SelectedPerson == null)
            {
                await DisplayAlert("Error", "Please select an employee to send email.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedPerson.Email))
            {
                await DisplayAlert("Error", "Selected employee has no email address.", "OK");
                return;
            }

            try
            {
                // EmailService will generate PDF automatically if needed
                bool success = await _emailService.SendScheduleEmail(SelectedPerson);

                if (success)
                {
                    await DisplayAlert(
                        "Email Ready",
                        $"Email to {SelectedPerson.Email} is ready to send. Please review and send it from your email client.",
                        "OK"
                    );
                }
                else
                {
                    await DisplayAlert("Error", "Failed to prepare email.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to send email: {ex.Message}", "OK");
            }
        }

        // INotifyPropertyChanged implementation
        public new event PropertyChangedEventHandler PropertyChanged;
        protected new void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}