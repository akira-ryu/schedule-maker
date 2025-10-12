using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace subcinctus_factorem.Employees
{
    public class Employee : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool _isSelected;
        private ObservableCollection<DaySchedule> _schedule;
        public ObservableCollection<DaySchedule> Schedule
        {
            get => _schedule;
            set { _schedule = value; OnPropertyChanged(); }
        }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class DaySchedule : INotifyPropertyChanged
    {
        public DateTime Date { get; set; }
        
        private string _position;
        public string Position
        {
            get => _position;
            set { _position = value; OnPropertyChanged(); }
        }

        private string _start;
        public string Start
        {
            get => _start;
            set { _start = value; OnPropertyChanged(); }
        }

        private string _end;
        public string End
        {
            get => _end;
            set { _end = value; OnPropertyChanged(); }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
