using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Scanner.Models
{
    public class ScanDocument : INotifyPropertyChanged
    {
        private string _Name;
        private string _Path;
        private string _Type;
        private DateTime _Date;

        public string Name
        {
            get => _Name;
            set => Set(ref _Name, value);
        }

        public string Path
        {
            get => _Path;
            set => Set(ref _Path, value);
        }

        public string Type
        {
            get => _Type;
            set => Set(ref _Type, value);
        }

        public DateTime Date
        {
            get => _Date;
            set => Set(ref _Date, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string PropertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

        protected bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value)) return false;

            field = value;
            OnPropertyChanged(PropertyName);
            return true;
        }
    }
}
