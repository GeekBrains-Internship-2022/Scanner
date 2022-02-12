using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Scanner.Models.Base;

namespace Scanner.Models
{
    public class ScanDocument : OnPropertyChangedClass
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
    }
}
