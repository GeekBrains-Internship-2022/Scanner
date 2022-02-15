using Scanner.Infrastructure.Commands;
using Scanner.interfaces;
using Scanner.Models;
using Scanner.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Scanner.ViewModels
{
    internal class ViewModelTestDB : ViewModel
    {
        private ObservableCollection<FileData> _fileDataObservableCollection;
        private ObservableCollection<ScannerDataTemplate> _ScannerDataTemplatesObservableCollection;
        private readonly IStore<FileData> _filedata;
        private readonly IStore<ScannerDataTemplate> _scannerData;

        public ObservableCollection<FileData> FileData
        {
            get { return _fileDataObservableCollection; }
            set { Set(ref _fileDataObservableCollection, value); }
        }
        public ObservableCollection<ScannerDataTemplate> ScannerDataTemplates
        {
            get { return _ScannerDataTemplatesObservableCollection; }
            set { Set(ref _ScannerDataTemplatesObservableCollection, value); }
        }

        #region Комманда для добавления в BD FileData
        private ICommand _CreateFileData;

        public ICommand CreateFileData => _CreateFileData
            ??= new LambdaCommand(OnCreateFileData, CanCreateFileData);

        private bool CanCreateFileData(object p) => true;

        private void OnCreateFileData(object p)
        {
            var value = (object[])p;
            var Path = (string)value[0];
            var Type = (string)value[1];
            var fd = _filedata.Add(new FileData());
            fd.FilePath = Path;
            fd.Document.DocumentType = Type;
            _filedata.Update(fd);
            FileData.Add(fd);
        }
        #endregion

        public ViewModelTestDB(IStore<FileData> __filedata, IStore<ScannerDataTemplate> __ScannerData)
        {
            _filedata = __filedata;
            _scannerData = __ScannerData;
            FileData = new ObservableCollection<FileData>(_filedata.GetAll());
            ScannerDataTemplates = new ObservableCollection<ScannerDataTemplate>(_scannerData.GetAll());
        }
    }
}
