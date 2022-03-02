using System;
using System.Collections.ObjectModel;
using Scanner.Models.Base;
using Scanner.ViewModels.Models;

namespace Scanner.Models
{
    public class ScanDocument : OnPropertyChangedClass
    {
        #region Name : string - имя документа

        private string _Name;

        public string Name
        {
            get => _Name;
            set => Set(ref _Name, value);
        }

        #endregion

        #region Path : string - путь хранения документа

        private string _Path;

        public string Path
        {
            get => _Path;
            set => Set(ref _Path, value);
        }

        #endregion

        #region Type : string - тип документа

        private string _Type;

        public string Type
        {
            get => _Type;
            set => Set(ref _Type, value);
        }

        #endregion

        #region Date : DateTime - дата

        private DateTime _Date;

        public DateTime Date
        {
            get => _Date;
            set => Set(ref _Date, value);
        }

        #endregion

        #region Metadata : ObservableCollection<Metadata> - коллекция метаданных

        private ObservableCollection<Metadata> _Metadata;

        public ObservableCollection<Metadata> Metadata
        {
            get => _Metadata;
            set => Set(ref _Metadata, value);
        }

        #endregion
    }
}
