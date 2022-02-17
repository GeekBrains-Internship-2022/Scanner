using Microsoft.Extensions.Logging;
using Scanner.Data.Stores.InDB;
using Scanner.interfaces;
using Scanner.interfaces.RabbitMQ;
using Scanner.Models;
using Scanner.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner.ViewModels
{
    internal class NewMainWindowViewModel : ViewModel
    {
        private readonly IStore<FileData> _FileDataInDB;
        private readonly IStore<ScannerDataTemplate> _DataTemplateInDB;
        private readonly ILogger<NewMainWindowViewModel> _Logger;
        private readonly IObserverService _Observer;
        private readonly IFileService _FileService;
        private readonly IRabbitMQService _RabbitMQService;

        private ObservableCollection<ScannerDataTemplate> _scannerDataTemplates;

        public ObservableCollection<ScannerDataTemplate> ScannerDataTemplates
        {
            get { return _scannerDataTemplates; }
            set { Set(ref _scannerDataTemplates, value); }
        }

        private ObservableCollection<FileData> _fileDatas;

        public ObservableCollection<FileData> FileDatas
        {
            get { return _fileDatas; }
            set { Set(ref _fileDatas, value); }
        }

        public NewMainWindowViewModel(IStore<FileData> __filedata, IStore<ScannerDataTemplate> __ScannerData,
            ILogger<NewMainWindowViewModel> __logger, IObserverService __Observer, 
            IFileService __FileService, IRabbitMQService __RabbitMQService)
        {
            _FileDataInDB = __filedata;
            _DataTemplateInDB = __ScannerData;
            _Logger = __logger;
            _Observer = __Observer;
            _FileService = __FileService;
            _RabbitMQService = __RabbitMQService;

            ScannerDataTemplates = new ObservableCollection<ScannerDataTemplate>(__ScannerData.GetAll());
            FileDatas = new ObservableCollection<FileData>(__filedata.GetAll());

            ObserverInitialize();
        }

        private async void ObserverInitialize()
        {
            _Observer.NotifyOnCreated += OnCreatedNotify;
            //_Observer.NotifyOnDeleted += OnDeletedNotify;
            //_Observer.NotifyOnRenamed += OnRenamedNotify;

            await _Observer.StartAsync();
        }

        private void OnCreatedNotify(string path)
        {
            throw new NotImplementedException();
        }
    }
}
