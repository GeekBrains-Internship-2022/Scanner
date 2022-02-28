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
        private readonly IStore<DocumentMetadata> _DBDocumentMetadataInDB;
        private readonly IStore<TemplateMetadata> _DBTemplateMetadataInDB;
        private readonly IStore<Document> _DBDocumentInDB;
        private readonly ILogger<MainWindowViewModel> _Logger;
        private readonly IConfiguration _Configuration;
        private readonly IObserverService _Observer;
        private readonly IFileService _FileService;
        private readonly IRabbitMQService _RabbitMQService;
        private readonly TestData _TestData;

        
        /// <summary>
        /// Список отсканированных документов
        /// </summary>
        public ObservableCollection<FileData> ScanDocuments { get; set; }                                //Список отсканированных документов

        /// <summary>
        /// Список отфильтрованных отсканированных документов
        /// </summary>
        public ObservableCollection<FileData> FilteredScanDocuments { get; set; } = new();                //Список отфильтрованных отсканированных документов
        
        /// <summary>
        /// Список шаблонов
        /// </summary>
        public ObservableCollection<ScannerDataTemplate> Templates { get; set; }                         //Список шаблонов

        /// <summary>
        /// Список найденных шаблонов по типу выбранного отсканированного документа
        /// </summary>
        public ObservableCollection<ScannerDataTemplate> FindTemplates { get; set; } = new();           //Список найденных шаблонов по типу выбранного отсканированного документа

        /// <summary>
        /// Список проиндексированных файлов
        /// </summary>
        public ObservableCollection<FileData> IndexedDocs { get; set; }                                 //Список проиндексированных файлов
        
        /// <summary>
        /// Список проверенных файлов
        /// </summary>
        public ObservableCollection<Document> VerifiedDocs { get; set; } = new();               //Список проверенных файлов

        /// <summary>
        /// Добавляемое администратором поле в шаблон
        /// </summary>
        public TemplateMetadata ExtraDataTemplate { get; set; } = new();                        //Добавляемое администратором поле в шаблон

        /// <summary>
        /// Список подпапок с отсканированными файлами
        /// </summary>
        public ObservableCollection<string> SubFolders { get; set; } = new();                   //Список подпапок с отсканированными файлами        

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

        #region ObservableCollection<DocumentMetadata> ExtraNoRequiredMetadatas - Список дополнительных необязательных метаданных
        private ObservableCollection<DocumentMetadata> _ExtraNoRequiredMetadatas = new ObservableCollection<DocumentMetadata>();        
        /// <summary>
        /// Список метаданных
        /// </summary>
        public ObservableCollection<DocumentMetadata> ExtraNoRequiredMetadatas                                         //Список дополнительных необязательных метаданных
        {
            get => _ExtraNoRequiredMetadatas;
            set
            {
                Set(ref _ExtraNoRequiredMetadatas, value);
            }
        }
        #endregion

        #region DocumentMetadata SelectedExtraNoRequiredMetadata - Выбранное оператором дополнительное необязательное поле метаданных
        private DocumentMetadata _SelectedExtraNoRequiredMetadata = new DocumentMetadata();
        /// <summary>
        /// Список метаданных
        /// </summary>
        public DocumentMetadata SelectedExtraNoRequiredMetadata                                         //Выбранное оператором дополнительное необязательное поле метаданных
        {
            get => _SelectedExtraNoRequiredMetadata;
            set
            {
                Set(ref _SelectedExtraNoRequiredMetadata, value);
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
                
                Metadatas.Clear();
                ExtraNoRequiredMetadatas.Clear();
                if (value != null)
                    foreach(var d in value.TemplateMetadata)
                    {
                        if (d.Required)
                            Metadatas.Add(new DocumentMetadata
                            {
                                Name = d.Name,
                                Data = null,
                            });
                        else
                            ExtraNoRequiredMetadatas.Add(new DocumentMetadata
                            {
                                Name = d.Name,
                                Data = null,
                            });
                    }
            }
        }
        #endregion

        #region SelectedEditTemplateAdmin : ScannerDataTemplate - выбранный шаблон для редактирования

        private ScannerDataTemplate _SelectedEditTemplateAdmin;
        public ScannerDataTemplate SelectedEditTemplateAdmin
        {
            get => _SelectedEditTemplateAdmin;
            set
            {
                Set(ref _SelectedEditTemplateAdmin, value);
                RowsSelectedEditTemplateAdmin = new ObservableCollection<TemplateMetadata>(value.TemplateMetadata);
            }
        }
        #endregion

        #region RowsSelectedEditTemplateAdmin : ObservableCollection<TemplateMetadata> - поля выбранного шаблона для редактирования

        private ObservableCollection<TemplateMetadata> _RowsSelectedEditTemplateAdmin;
        public ObservableCollection<TemplateMetadata> RowsSelectedEditTemplateAdmin
        {
            get => _RowsSelectedEditTemplateAdmin;
            set
            {
                Set(ref _RowsSelectedEditTemplateAdmin, value);
                if(value != null)
                    SelectedEditTemplateAdmin.TemplateMetadata = value;
            }
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
            set
            {
                Set(ref _SelectedIndexedDoc, value);
                if(value is not null)
                    MetadataSelectedIndexedDoc = new ObservableCollection<DocumentMetadata>(value.Document.Metadata);
            }
        }

        #endregion

        #region MetadataSelectedIndexedDoc : ObservableCollection<DocumentMetadata> - список метаданных выбранного индексированныго документа

        private ObservableCollection<DocumentMetadata> _MetadataSelectedIndexedDoc;

        public ObservableCollection<DocumentMetadata> MetadataSelectedIndexedDoc
        {
            get => _MetadataSelectedIndexedDoc;
            set => Set(ref _MetadataSelectedIndexedDoc, value);
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
            IStore<Document> __Document,
            ILogger<MainWindowViewModel> __logger,
            IConfiguration __configuration,
            IObserverService __observer,
            IFileService __fileService,
            IRabbitMQService __rabbitMQService)
        {
            _DBFileDataInDB = __filedata;                       // Подлючение к базе FileData - хранение информации о файлах
            _DBDataTemplateInDB = __ScannerData;                // Подключение к базе ScannerDataTemplate - хранение шаблонов
            _DBDocumentMetadataInDB = __DocumentMetadataDB;     // Подключение к базе DocumentMetadata - храненние метаданных документов
            _DBTemplateMetadataInDB = __TemplateMetadata;       // Подключение к базе TemplateMetadata  - хранение названий полей в шаблоне
            _DBDocumentInDB = __Document;                       // Подключение к базе Document - хранение документов
            _Logger = __logger;
            _Configuration = __configuration;
            _Observer = __observer;
            _FileService = __fileService;
            _RabbitMQService = __rabbitMQService;

            ScanDocuments = new();
            
            _TestData = new TestData();

            IndexedDocs = new ObservableCollection<FileData>(_DBFileDataInDB.GetAll().Where(i => i.Indexed));

            var ScannerDataTemplatesInDB = new ObservableCollection<ScannerDataTemplate>(_DBDataTemplateInDB.GetAll());
            var FileDatasInDB = new ObservableCollection<FileData>(_DBFileDataInDB.GetAll());

            GetFiles();

            var scannerDataTemplates = new ObservableCollection<ScannerDataTemplate>(_DBDataTemplateInDB.GetAll());
            if(ScannerDataTemplatesInDB.Count == 0)
                foreach(var t in _TestData.DataTemplates)
                {                    
                    _DBDataTemplateInDB.Add(t);
                }
            else
                foreach (var t in _TestData.DataTemplates)
                {
                    if (ScannerDataTemplatesInDB.FirstOrDefault(st => st.DocumentType == t.DocumentType) is null)
                        _DBDataTemplateInDB.Add(t);
                }

            //Templates = _TestData.DataTemplates;
            Templates = new ObservableCollection<ScannerDataTemplate>(_DBDataTemplateInDB.GetAll());

            _TestData.FilesDatas = ScanDocuments;

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
            var metadata = new Collection<DocumentMetadata>();

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
            var file = SelectedDocument;
            
            var path = _Configuration["Directories:StorageDirectory"];
            path = Path.GetFullPath(path);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var s = Path.Combine(path, Guid.NewGuid().ToString("N") + ".pdf");

            if (file is null) return;

            var oldPath = file.FilePath;
            file.Document.IndexingDate = DateTime.Now;
            file.FilePath = s;
            file.Indexed = true;
            File.Copy(oldPath, s);
                
            _TestData.FilesDatas.Add(file);

            foreach (var m in Metadatas)
            {
                file.Document.Metadata.Add(new DocumentMetadata
                {
                    Name = m.Name,
                    Data = m.Data,
                });
            }

            var fileInDB = _DBFileDataInDB.Add(file);

            IndexedDocs.Add(fileInDB);
            ScanDocuments.Remove(file);
            FilteredScanDocuments.Remove(file);

            SelectedDocument = null;
            Metadatas.Clear();

            IsNew = false;
            Status = "Готов";

            //File.Delete(oldPath);

            //SelectedDocument = FilteredScanDocuments.Next                 //Заглушка. Выбор следующего документа при сохранении (похоже нужно сначала отсортировать список)
        }

        private bool CanSaveFileCommandExecute(object p)
        {
            bool result = false;

            if (Metadatas.Count > 0 && SelectedDocument != null)
            {
                var v = Metadatas.FirstOrDefault(m => m.Data == null || m.Data.Length == 0 || m.Data == " ");
                if (v != null)
                    result = false;
                else result = true;
            }

            return result;
        }

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
        private bool CanAddExtraDataToDocumentExecute(object p) => SelectedExtraNoRequiredMetadata is not null;

        private void AddDataToDocument()
        {
            if (SelectedExtraNoRequiredMetadata is null) return;     //  На случай если затупит UI и не сработает условие команды
            if (SelectedExtraNoRequiredMetadata.Name != null && SelectedDocument != null && !Metadatas.Contains(SelectedExtraNoRequiredMetadata))
                Metadatas.Add(SelectedExtraNoRequiredMetadata);
        }

        #endregion

        #region DeleteExtraDataFromDocument - Команда удаления элемента метаднных из списка метаданных документа - заглушка

        private ICommand _DeleteExtraDataFromDocument;
        public ICommand DeleteExtraDataFromDocument => _DeleteExtraDataFromDocument
            ??= new LambdaCommand(OnDeleteExtraDataFromDocumentExecuted, CanDeleteExtraDataFromDocumentExecute);

        private void OnDeleteExtraDataFromDocumentExecuted(object p) => DeleteDataFromDocument(p);
        private bool CanDeleteExtraDataFromDocumentExecute(object p)
        {
            bool result = false;
            if (p is null) result = true;
            if (p is DocumentMetadata meta)
            {
                var v = SelectedTemplate?.TemplateMetadata.FirstOrDefault(t => t.Name == meta.Name);
                if (v == null)
                {
                    result = false;
                    return false;
                }
                if (!v.Required)
                    result = true;
            }

            return result;
        }

        private void DeleteDataFromDocument(object p)
        {
            if (p is DocumentMetadata meta)
            {
                Metadatas.Remove(meta);
                /*var temp = SelectedTemplateInOP?.TemplateMetadata.FirstOrDefault(o => o.Name == meta.Name);
                if (temp is null) return;
                if (!TemplateMetadatas.Contains(temp))
                {
                    TemplateMetadatas.Add(temp);
                }*/
            }
            return;
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
            var doc = _DBFileDataInDB.GetById(SelectedIndexedDoc.Id);
            doc.Indexed = false;
            doc.DocumentName = "(Доработать)" + doc.DocumentName;
            SelectedIndexedDoc = null;
            IndexedDocs.Remove(doc);
            _DBFileDataInDB.Update(doc);
            ScanDocuments.Add(doc);
            FilteredScanDocuments.Add(doc);
        }

        private bool CanAdminReworkCommandExecute(object p) => SelectedIndexedDoc is not null;

        #endregion

        #endregion        
    }
}