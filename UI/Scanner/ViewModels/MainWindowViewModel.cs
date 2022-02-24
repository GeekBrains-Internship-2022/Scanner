using System;
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
        private readonly IStore<FileData> _DBFileDataInDB;
        private readonly IStore<ScannerDataTemplate> _DBDataTemplateInDB;
        private readonly IStore<DocumentMetadata> _DBdocumentMetadata;
        private readonly IStore<TemplateMetadata> _DBtemplateMetadata;
        private readonly ILogger<MainWindowViewModel> _Logger;
        private readonly IConfiguration _Configuration;
        private readonly IObserverService _Observer;
        private readonly IFileService _FileService;
        private readonly IRabbitMQService _RabbitMQService;
        private readonly TestData _TestData = new TestData();

        /// <summary>
        /// Список отсканированных документов
        /// </summary>
        public ObservableCollection<FileData> ScanDocuments { get; set; } = new();          //Список отсканированных документов

        /// <summary>
        /// Список отфильтрованных отсканированных документов
        /// </summary>
        public ObservableCollection<FileData> FilteredScanDocuments { get; set; } = new();  //Список отфильтрованных отсканированных документов

        /// <summary>
        /// Список шаблонов
        /// </summary>
        public ObservableCollection<ScannerDataTemplate> Templates { get; set; } = new();                  //Список шаблонов

        /// <summary>
        /// Список найденных шаблонов по типу выбранного отсканированного документа
        /// </summary>
        public ObservableCollection<ScannerDataTemplate> FindTemplates { get; set; } = new();              //Список найденных шаблонов по типу выбранного отсканированного документа

        /// <summary>
        /// Список проиндексированных файлов
        /// </summary>
        public ObservableCollection<FileData> IndexedDocs { get; set; } = new();            //Список проиндексированных файлов

        /// <summary>
        /// Список проверенных файлов
        /// </summary>
        public ObservableCollection<Document> VerifiedDocs { get; set; } = new();               //Список проверенных файлов

        /// <summary>
        /// Добавляемое поле в шаблон
        /// </summary>
        public TemplateMetadata ExtraDataTemplate { get; set; } = new();                        //Добавляемое поле в шаблон

        /// <summary>
        /// Список подпапок с отсканированными файлами
        /// </summary>
        public ObservableCollection<string> SubFolders { get; set; } = new();                   //Список подпапок с отсканированными файлами

        /// <summary>
        /// Список полей Data, соответствующих выбранному шаблону SelectedTemplate для SelectedDocument
        /// </summary>
        public ObservableCollection<DocumentMetadata> DataListSelectedDocument { get; set; } = new();     //Список полей Data, соответствующих выбранному шаблону SelectedTemplate для SelectedDocument

        # region ObservableCollection<DocumentMetadata> Metadatas - Список метаданных
        private ObservableCollection<DocumentMetadata> _Metadatas = new ObservableCollection<DocumentMetadata>();
        /// <summary>
        /// Список метаданных
        /// </summary>
        public ObservableCollection<DocumentMetadata> Metadatas                                         //Список метаданных
        {
            get => _Metadatas;
            set
            {
                Set(ref _Metadatas, value);
            }
        }
        #endregion


        #region IsConnected : bool - индикатор подключения

        private bool _IsConnected = true;

        public bool IsConnected
        {
            get => _IsConnected;
            set => Set(ref _IsConnected, value);
        }

        #endregion        

        #region SelectedDocument : FileData - выбранный документ

        private FileData _SelectedDocument;

        public FileData SelectedDocument
        {
            get => _SelectedDocument;
            set
            {
                Set(ref _SelectedDocument, value);
                if (Templates != null)
                    foreach(var t in Templates)
                    {
                        if (value != null)
                            SelectedTemplate = Templates.FirstOrDefault(t => t.DocumentType.ToLower().Contains(value.Document.DocumentType.ToLower()));
                            //if(value.Type.ToLower().Contains(t.Name.ToLower()))
                            //{
                            //    SelectedTemplate = t;
                            //    return;
                            //}
                            //else
                            //{
                            //    SelectedTemplate = null;
                            //}
                    }                    
            }
        }
        #endregion

        #region SelectedTemplate : Template - выбранный шаблон для SelectedDocument
        private ScannerDataTemplate _SelectedTemplate;
        public ScannerDataTemplate SelectedTemplate
        {
            get => _SelectedTemplate;
            set
            {
                Set(ref _SelectedTemplate, value);
                DataListSelectedDocument.Clear();
                if(value != null)
                    foreach(var d in value.TemplateMetadata)
                    {
                        //DataListSelectedDocument.Add(d.Name);
                        Metadatas.Add(new DocumentMetadata
                        {
                            Name = d.Name,
                            Data = null,                            
                        });
                    }
            }
        }
        #endregion

        #region SelectedEditTemplateAdmin : Template - выбранный шаблон для редактирования

        private ScannerDataTemplate _SelectedEditTemplateAdmin;
        public ScannerDataTemplate SelectedEditTemplateAdmin
        {
            get => _SelectedEditTemplateAdmin;
            set => Set(ref _SelectedEditTemplateAdmin, value);
        }
        #endregion

        #region NameExtraDataTemplate : Metadata - дополнительное поле данных для добавления в шаблон

        private string _NameExtraDataTemplate;
        public string NameExtraDataTemplate
        {
            get { return _NameExtraDataTemplate; }
            set { _NameExtraDataTemplate = value; }
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

        #region IsNew : bool - появились новые файлы

        private bool _IsNew = false;

        public bool IsNew
        {
            get => _IsNew;
            set => Set(ref _IsNew, value);
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

        #region SelectedIndexedDoc : FileData - выбранный индексированный документ

        private FileData _SelectedIndexedDoc;

        public FileData SelectedIndexedDoc
        {
            get => _SelectedIndexedDoc;
            set => Set(ref _SelectedIndexedDoc, value);
        }

        #endregion

        #region SelectedFilterItem : string - выбраный тип документа в фильтре

        private string _selectedFilterItem;
        public string SelectedFilterItem
        {
            get => _selectedFilterItem;
            set
            {                
                Set(ref _selectedFilterItem, value);

                if (value == null || value == "" || value.Contains("Не выбрано"))
                {
                    FilteredScanDocuments.Clear();
                    foreach (var d in ScanDocuments)
                    {
                        FilteredScanDocuments.Add(d);
                    }
                    
                }
                else
                {
                    FilteredScanDocuments.Clear();

                    foreach (var d in ScanDocuments)                            //Сортировка по типу
                    {
                        if (value.ToLower() == d.Document.DocumentType.ToLower())
                            FilteredScanDocuments.Add(d);
                    }
                }
            }
        }

        #endregion

        public MainWindowViewModel(IStore<FileData> __filedata,
            IStore<ScannerDataTemplate> __ScannerData,
            IStore<DocumentMetadata> __DocumentMetadataDB,
            IStore<TemplateMetadata> __TemplateMetadata,
            ILogger<MainWindowViewModel> __logger,
            IConfiguration __configuration,
            IObserverService __observer,
            IFileService __fileService,
            IRabbitMQService __rabbitMQService)
        {
            _DBFileDataInDB = __filedata;                   // Подлючение к базе FileData - хранение информации о файлах
            _DBDataTemplateInDB = __ScannerData;            // Подключение к базе ScannerDataTemplate - хранение шаблонов
            _DBdocumentMetadata = __DocumentMetadataDB;     // Подключение к базе DocumentMetadata - храненние метаданных документов
            _DBtemplateMetadata = __TemplateMetadata;       // Подключение к базе TemplateMetadata  - хранение названий полей в шаблоне
            _Logger = __logger;
            _Configuration = __configuration;
            _Observer = __observer;
            _FileService = __fileService;
            _RabbitMQService = __rabbitMQService;

            //Templates = _TestData.Templates;
            ObservableCollection<ScannerDataTemplate> ScannerDataTemplates = new ObservableCollection<ScannerDataTemplate>(__ScannerData.GetAll());
            GetFiles();
            foreach(var d in ScanDocuments)
                _DBFileDataInDB.Add(d);
            FilteredScanDocuments = new ObservableCollection<FileData>(ScanDocuments);

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
            var document = ScanDocuments.FirstOrDefault(d => d.FilePath == oldPath);

            if (document is null)
                return;

            var fileName = new FileInfo(currentPath).Name;

            document.DocumentName = fileName;
            document.FilePath = currentPath;
        }

        private void OnCreatedNotify(string message)
        {
            var document = ScanDocuments.FirstOrDefault(d => d.FilePath == message);

            if (ScanDocuments.Contains(document))
                throw new DuplicateNameException(message);

            Application.Current.Dispatcher.Invoke(() => { ScanDocuments.Add(GetDocumentByPath(message)); });

            //  Лампочка
            Status = "Появились новые документы для индексации!!!";
            IsNew = true;
        }

        private void OnDeletedNotify(string message)
        {
            var document = ScanDocuments.FirstOrDefault(d => d.FilePath == message);

            if (document is null)
                return;

            Application.Current.Dispatcher.Invoke(() => { ScanDocuments.Remove(document); });
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
            {
                files.AddRange(Directory.GetFiles(dir));
                string[] str = dir.Split('\\');
                SubFolders.Add(dir.Split('\\')[str.Length - 1]);    // Получение списка подпапок, содержащихся в горячей папке
            }

            foreach (var file in files)
                ScanDocuments.Add(GetDocumentByPath(file));         // Получение списка отсканированных документов
        }

        private FileData GetDocumentByPath(string file)
        {
            var fileInfo = new FileInfo(file);
            var type = fileInfo.DirectoryName?.Split('\\')[^1];
            var metadata = Metadatas.Where(t => t.Name == type) as ICollection<DocumentMetadata>;

            var document = new Document { DocumentType = type, IndexingDate = DateTime.MinValue, Metadata = metadata };

            return new FileData
            {
                DocumentName = fileInfo.Name,
                FilePath = file,
                Description = "",
                DateAdded = fileInfo.CreationTime,
                Indexed = false,
                Checked = false,
                Document = document,
            };

            /*return new ScanDocument
            {
                Name = fileInfo.Name,
                Date = fileInfo.CreationTime,
                Path = file,
                Type = type,
                Metadata = metadata,
            };*/
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

        #region OpenTestBDCommand - команда открыть настройки 

        private ICommand _OpenTestBDCommand;

        public ICommand OpenTestBDCommand => _OpenTestBDCommand
            ??= new LambdaCommand(OnOpenTestBDCommandExecuted, CanOpenTestBDCommandExecute);

        private void OnOpenTestBDCommandExecuted(object p)
        {
            var testBDWindow = new testDB();
            testBDWindow.ShowDialog();
        }

        private bool CanOpenTestBDCommandExecute(object p) => true;

        #endregion

        #region CloseAppCommand - команда закрыть приложение

        private ICommand _CloseAppCommand;

        public ICommand CloseAppCommand => _CloseAppCommand
            ??= new LambdaCommand(OnCloseAppCommandExecuted, CanCloseAppCommandExecute);

        private void OnCloseAppCommandExecuted(object p) => Application.Current.Shutdown();

        private bool CanCloseAppCommandExecute(object p) => true;

        #endregion

        #region SaveFileCommand - Команда сохранения файла после индексирования - заглушка

        private ICommand _SaveFileCommand;
        public ICommand SaveFileCommand => _SaveFileCommand
            ??= new LambdaCommand(OnSaveFileCommandExecuted, CanSaveFileCommandExecute);

        private void OnSaveFileCommandExecuted(object p) => SaveFile();


        /// <summary>
        /// Сохранение файла
        /// </summary>
        private void SaveFile()
        {
            var doc = SelectedDocument;
            SelectedDocument = null;
            var path = _Configuration["Directories:StorageDirectory"];
            path = Path.GetFullPath(path);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var s = Path.Combine(path, Guid.NewGuid().ToString("N") + ".pdf");
            var oldPath = doc.FilePath;

            var metadata = new ObservableCollection<DocumentMetadata>();

            foreach (var m in Metadatas)
                metadata.Add(new DocumentMetadata { Name = m.Name, Data = m.Data, });

            doc.Document.Metadata = metadata;
            doc.FilePath = s;
            File.Copy(oldPath, s);
            IndexedDocs.Add(doc);
            ScanDocuments.Remove(doc);
            FilteredScanDocuments.Remove(doc);

            IsNew = false;
            Status = "Готов";

            //File.Delete(oldPath);

            //SelectedDocument = FilteredScanDocuments.Next                 //Заглушка. Выбор следующего документа при сохранении (похоже нужно сначала отсортировать список)
        }

        private bool CanSaveFileCommandExecute(object p) => true;

        #endregion

        #region AddExtraMetadataTemplate - Команда добавления поля в редактируемый шаблон

        private ICommand _AddExtraMetadataTemplate;
        public ICommand AddExtraMetadataTemplate => _AddExtraMetadataTemplate
            ??= new LambdaCommand(OnAddExtraMetadataTemplateExecuted, CanAddExtraMetadataTemplateExecute);

        private void OnAddExtraMetadataTemplateExecuted(object p) => AddExtraMetadata();
        private bool CanAddExtraMetadataTemplateExecute(object p) => true;

        private void AddExtraMetadata()
        {
            ExtraDataTemplate = new TemplateMetadata { Name = NameExtraDataTemplate, Required = false };
            SelectedEditTemplateAdmin.TemplateMetadata.Add(ExtraDataTemplate);
        }

        #endregion

        #region AddExtraDataToDocument - Команда добавления поля Data в редактируемый документ - заглушка

        private ICommand _AddExtraDataToDocument;
        public ICommand AddExtraDataToDocument => _AddExtraDataToDocument
            ??= new LambdaCommand(OnAddExtraDataToDocumentExecuted, CanAddExtraDataToDocumentExecute);

        private void OnAddExtraDataToDocumentExecuted(object p) => AddDataToDocument();
        private bool CanAddExtraDataToDocumentExecute(object p) => true;

        private void AddDataToDocument()
        {
            TemplateMetadata metadata = new TemplateMetadata();
            SelectedDocument.Document.Metadata.Add(new DocumentMetadata { Name = metadata.Name,});
        }

        #endregion

        #region SaveEditTemplateToBD - команда сохранения шаблона в базу - заглушка

        private ICommand _SaveEditTemplateToBD;
        public ICommand SaveEditTemplateToBD => _SaveEditTemplateToBD
            ??= new LambdaCommand(OnSaveEditTemplateToBDExecuted, CanSaveEditTemplateToBDExecute);

        private void OnSaveEditTemplateToBDExecuted(object p) => SaveEditTemplate();
        private bool CanSaveEditTemplateToBDExecute(object p) => true;

        private void SaveEditTemplate()
        {
            //_TestData.Templates.Add(SelectedEditTemplateAdmin);          //Необходимо сделать провеку на уже имеющийся шаблон, если есть, то предложить переименовать, если нет, то сохраняем
        }

        #endregion RemoveTemplateFromBD - команда удаления шаблона из базы - заглушка

        #region RemoveTemplateFromBD - команда удаления шаблона из бд

        private ICommand _RemoveTemplateFromBD;
        public ICommand RemoveTemplateFromBD => _RemoveTemplateFromBD
            ??= new LambdaCommand(OnRemoveTemplateFromBDExecuted, CanRemoveTemplateFromBDExecute);

        private void OnRemoveTemplateFromBDExecuted(object p) => RemoveTemplate();
        private bool CanRemoveTemplateFromBDExecute(object p) => true;

        private void RemoveTemplate()
        {
            if (SelectedEditTemplateAdmin != null)
                if (MessageBox.Show($"Вы уверены что хотите удалить {SelectedEditTemplateAdmin.DocumentType}?", "Удалить",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    //_TestData.Templates.Remove(SelectedEditTemplateAdmin);
                    return;
        }

        #endregion

        #region CreateNewTemplate - команда создания нового шаблона - заглушка

        private ICommand _CreateNewTemplate;
        public ICommand CreateNewTemplate => _CreateNewTemplate
            ??= new LambdaCommand(OnCreateNewTemplateExecuted, CanCreateNewTemplateExecute);
        private void OnCreateNewTemplateExecuted(object p) => CreateTemplate();
        private bool CanCreateNewTemplateExecute(object p) => true;

        private void CreateTemplate()
        {
            ScannerDataTemplate template = new ScannerDataTemplate { DocumentType = "Новый шаблон", };
            Templates.Add(template);
        }

        #endregion

        #region NextFileCommand - Команда выбора следующего файла - заглушка

        private ICommand _NextFileCommand;
        public ICommand NextFileCommand => _NextFileCommand
            ??= new LambdaCommand(OnNextFileCommandExecuted, CanNextFileCommandExecute);

        private void OnNextFileCommandExecuted(object p) => NextFile();
        private bool CanNextFileCommandExecute(object p) => true;

        private void NextFile()
        {

        }

        #endregion

        #region AdminFinishCommand - Команда завершения обработки - заглушка

        private ICommand _AdminFinishCommand;

        public ICommand AdminFinishCommand => _AdminFinishCommand
            ??= new LambdaCommand(OnAdminFinishCommandExecuted, CanAdminFinishCommandExecute);

        private void OnAdminFinishCommandExecuted(object p)
        {

        }

        private bool CanAdminFinishCommandExecute(object p) => SelectedIndexedDoc is not null;

        #endregion

        #region AdminReworkCommand - Команда отправки на доработку

        private ICommand _AdminReworkCommand;        
        public ICommand AdminReworkCommand => _AdminReworkCommand
            ??= new LambdaCommand(OnAdminReworkCommandExecuted, CanAdminReworkCommandExecute);

        private void OnAdminReworkCommandExecuted(object p)
        {
            var doc = SelectedIndexedDoc;
            doc.DocumentName = "(Доработать)" + doc.DocumentName;
            SelectedIndexedDoc = null;
            IndexedDocs.Remove(doc);
            ScanDocuments.Add(doc);
        }

        private bool CanAdminReworkCommandExecute(object p) => SelectedIndexedDoc is not null;

        #endregion

        #endregion        
    }
}