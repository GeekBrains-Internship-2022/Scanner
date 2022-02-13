using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Scanner.Data;
using Scanner.Infrastructure.Commands;
using Scanner.interfaces;
using Scanner.interfaces.RabbitMQ;
using Scanner.Models;
using Scanner.ViewModels.Base;
using Scanner.ViewModels.Models;
using Scanner.Views.Windows;


namespace Scanner.ViewModels
{
    class MainWindowViewModel : ViewModel
    {
        private readonly ILogger<MainWindowViewModel> _Logger;
        private readonly IConfiguration _Configuration;

        private readonly IObserverService _Observer;
        private readonly IFileService _FileService;
        private readonly IRabbitMQService _RabbitMQService;
        private readonly TestData _TestData = new TestData();

        public ObservableCollection<ScanDocument> Documents { get; set; } = new();
        public ObservableCollection<Template> Templates { get; set; } = new();
        public ObservableCollection<Metadata> Metadatas { get; set; } = new();

        #region IsConnected : bool - индикатор подключения

        private bool _IsConnected = true;

        public bool IsConnected
        {
            get => _IsConnected;
            set => Set(ref _IsConnected, value);
        }

        #endregion        

        #region SelectedDocument : ScanDocument - выбранный документ

        private ScanDocument _SelectedDocument;

        public ScanDocument SelectedDocument
        {
            get => _SelectedDocument;
            set => Set(ref _SelectedDocument, value);
        }
        #endregion

        #region SelectedTemplate : Template - выбранный шаблон
        private Template _SelectedTemplate;
        public Template SelectedTemplate
        {
            get => _SelectedTemplate;
            set => Set(ref _SelectedTemplate, value);
        }
        

        #endregion

        #region Status : string - статус

        private string _Status = "Готов";

        public string Status
        {
            get => _Status;
            set => Set(ref _Status, value);
        }

        #endregion

        #region SelectedSorting : string - выбранная сортировка

        private string _SelectedSorting;

        public string SelectedSorting
        {
            get => _SelectedSorting;
            set => Set(ref _SelectedSorting, value);
        }

        #endregion

        public MainWindowViewModel(ILogger<MainWindowViewModel> logger, IConfiguration configuration,
            IObserverService observer, IFileService fileService, IRabbitMQService rabbitMQService)
        {
            _Logger = logger;
            _Configuration = configuration;
            _Observer = observer;
            _FileService = fileService;
            _RabbitMQService = rabbitMQService;

            Templates = _TestData.Templates;
            /*Templates =  new ObservableCollection<Template>
            {
                new Template()
                {
                    Name = "Паспорт",
                    Metadata =  new ObservableCollection<Metadata>
                    {
                        new Metadata
                        {
                            Name ="Номер",
                            Required = true,                            
                        }
                    }
                },
                new Template()
                {
                    Name = "Свидетельство",
                    Metadata =  new ObservableCollection<Metadata>
                    {
                        new Metadata
                        {
                            Name ="Номер",
                            Required = true,                            
                        }
                    }
                },
            };*/

            ObserverInitialize();
        }

        #region Observer

        private async void ObserverInitialize()
        {
            _Observer.NotifyOnCreated += OnCreatedNotify;
            _Observer.NotifyOnDeleted += OnDeletedNotify;
            _Observer.NotifyOnRenamed += OnRenamedNotify;

            await _Observer.StartAsync();
        }

        private void OnRenamedNotify(string oldPath, string currentPath)
        {
            var document = Documents.FirstOrDefault(d => d.Path == oldPath);

            if (document is null)
                return;

            var fileName = new FileInfo(currentPath).Name;

            document.Name = fileName;
            document.Path = currentPath;
        }

        private void OnCreatedNotify(string message)
        {
            var document = Documents.FirstOrDefault(d => d.Path == message);

            if (Documents.Contains(document))
                throw new DuplicateNameException(message);

            Application.Current.Dispatcher.Invoke(() => { Documents.Add(GetDocumentByPath(message)); });

            //  Лампочка
            Status = "Появились новые документы для индексации!!!";
        }

        private void OnDeletedNotify(string message)
        {
            var document = Documents.FirstOrDefault(d => d.Path == message);

            if (document is null)
                return;

            Application.Current.Dispatcher.Invoke(() => { Documents.Remove(document); });
        }

        #endregion

        private void GetFiles()
        {
            var path = _Configuration["Directories:ObserverPath"];

            if (string.IsNullOrEmpty(path))
                return;

            if (!Path.IsPathFullyQualified(path))
                path = Path.GetFullPath(path);

            var directories = Directory.GetDirectories(path);
            var files = new List<string>();
            foreach (var dir in directories)
                files.AddRange(Directory.GetFiles(dir));

            foreach (var file in files)
                Documents.Add(GetDocumentByPath(file));
        }

        private ScanDocument GetDocumentByPath(string file)
        {
            var fileInfo = new FileInfo(file);

            return new ScanDocument
            {
                Name = fileInfo.Name,
                Date = fileInfo.CreationTime,
                Path = file,
                Type = fileInfo.DirectoryName.Split('\\')[^1]
            };
        }

        #region Команды

        #region GetFilesCommand - Команда на получение списка файлов

        private ICommand _GetFilesCommand;

        public ICommand GetFilesCommand => _GetFilesCommand
            ??= new LambdaCommand(OnGetFilesCommandExecuted, CanGetFilesCommandExecute);

        private void OnGetFilesCommandExecuted(object p) => GetFiles();

        private bool CanGetFilesCommandExecute(object p) => true;

        #endregion

        #region OpenSettingsCommand - команда открыть настройки 

        private ICommand _OpenSettingsCommand;

        public ICommand OpenSettingsCommand => _OpenSettingsCommand
            ??= new LambdaCommand(OnOpenSettingsCommandExecuted, CanOpenSettingsCommandExecute);

        private void OnOpenSettingsCommandExecuted(object p)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        private bool CanOpenSettingsCommandExecute(object p) => true;

        #endregion

        #region CloseAppCommand - команда закрыть приложение

        private ICommand _CloseAppCommand;

        public ICommand CloseAppCommand => _CloseAppCommand
            ??= new LambdaCommand(OnCloseAppCommandExecuted, CanCloseAppCommandExecute);

        private void OnCloseAppCommandExecuted(object p) => Application.Current.Shutdown();

        private bool CanCloseAppCommandExecute(object p) => true;

        #endregion
        
        #endregion
    }
}