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
using Scanner.Infrastructure.Commands;
using Scanner.interfaces;
using Scanner.interfaces.RabbitMQ;
using Scanner.Models;
using Scanner.ViewModels.Base;
using Scanner.Views.Windows;

namespace Scanner.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private readonly IStore<FileData> _DBFileDataInDB;
        private readonly IStore<ScannerDataTemplate> _DBDataTemplateInDB;
        private readonly ILogger<MainWindowViewModel> _Logger;
        private readonly IConfiguration _Configuration;
        private readonly IObserverService _Observer;
        private readonly IRabbitMQService _RabbitMQService;

        /// <summary>Список отсканированных документов</summary>
        public ObservableCollection<FileData> ScanDocuments { get; set; }                                //Список отсканированных документов

        /// <summary>Список отфильтрованных отсканированных документов</summary>
        public ObservableCollection<FileData> FilteredScanDocuments { get; set; }                        //Список отфильтрованных отсканированных документов
        
        /// <summary>Список шаблонов</summary>
        public ObservableCollection<ScannerDataTemplate> Templates { get; set; }                         //Список шаблонов

        /// <summary>Список проиндексированных файлов</summary>
        public ObservableCollection<FileData> IndexedDocs { get; set; }                                 //Список проиндексированных файлов

        /// <summary>Добавляемое администратором поле в шаблон</summary>
        public TemplateMetadata ExtraDataTemplate { get; set; } = new();                                //Добавляемое администратором поле в шаблон

        /// <summary>Список подпапок с отсканированными файлами</summary>
        public ObservableCollection<string> SubFolders { get; set; } = new();                           //Список подпапок с отсканированными файлами        

        # region ObservableCollection<DocumentMetadata> Metadatas - Список метаданных
        private ObservableCollection<DocumentMetadata> _Metadatas = new();
        /// <summary>
        /// Список метаданных
        /// </summary>
        public ObservableCollection<DocumentMetadata> Metadatas                                         //Список метаданных
        {
            get => _Metadatas;
            set => Set(ref _Metadatas, value);
        }
        #endregion

        #region ObservableCollection<DocumentMetadata> ExtraNoRequiredMetadatas - Список дополнительных необязательных метаданных
        private ObservableCollection<DocumentMetadata> _ExtraNoRequiredMetadatas = new();
        /// <summary>
        /// Список метаданных
        /// </summary>
        public ObservableCollection<DocumentMetadata> ExtraNoRequiredMetadatas                                         //Список дополнительных необязательных метаданных
        {
            get => _ExtraNoRequiredMetadatas;
            set => Set(ref _ExtraNoRequiredMetadatas, value);
        }
        #endregion

        #region DocumentMetadata SelectedExtraNoRequiredMetadata - Выбранное оператором дополнительное необязательное поле метаданных
        private DocumentMetadata _SelectedExtraNoRequiredMetadata = new();
        /// <summary>
        /// Список метаданных
        /// </summary>
        public DocumentMetadata SelectedExtraNoRequiredMetadata                                         //Выбранное оператором дополнительное необязательное поле метаданных
        {
            get => _SelectedExtraNoRequiredMetadata;
            set => Set(ref _SelectedExtraNoRequiredMetadata, value);
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
                if (SelectedDocument is null) return;

                if (SelectedDocument.Indexed)
                {
                    SelectedTemplate =
                        Templates.FirstOrDefault(t => t.DocumentType == SelectedDocument.Document.DocumentType);
                    Metadatas = new ObservableCollection<DocumentMetadata>(SelectedDocument.Document.Metadata);
                    return;
                }

                if (Templates != null)
                    SelectedTemplate = Templates.FirstOrDefault(t =>
                        t.DocumentType.ToLower().Contains(value.Document.DocumentType.ToLower()));
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

                if (value == null) return;

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
                if(value != null)
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

        #region RowSelectedEditTemplateAdmin : TemplateMetadata - выбранное поле выбранного шаблона для редактирования

        private TemplateMetadata _RowSelectedEditTemplateAdmin;
        public TemplateMetadata RowSelectedEditTemplateAdmin
        {
            get => _RowSelectedEditTemplateAdmin;
            set => Set(ref _RowSelectedEditTemplateAdmin, value);
        }
        #endregion

        #region NameExtraDataTemplate : Metadata - дополнительное поле данных для добавления в шаблон

        private string _NameExtraDataTemplate;
        public string NameExtraDataTemplate
        {
            get => _NameExtraDataTemplate;
            set => Set(ref _NameExtraDataTemplate, value);
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

        private bool _IsNew;

        public bool IsNew
        {
            get => _IsNew;
            set => Set(ref _IsNew, value);
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

                if (string.IsNullOrEmpty(value) || value.Contains("Не выбрано"))
                {
                    FilteredScanDocuments.Clear();
                    foreach (var d in ScanDocuments) 
                        FilteredScanDocuments.Add(d);

                }
                else
                {
                    FilteredScanDocuments.Clear();

                    foreach (var d in ScanDocuments)                            //Сортировка по типу
                        if (value.ToLower() == d.Document.DocumentType.ToLower())
                            FilteredScanDocuments.Add(d);
                }
            }
        }

        #endregion

        public MainWindowViewModel(IStore<FileData> filedata, IStore<ScannerDataTemplate> scannerData,
            ILogger<MainWindowViewModel> logger, IConfiguration configuration, IObserverService observer,
            IRabbitMQService rabbitMqService)
        {
            _DBFileDataInDB = filedata;                       // Подлючение к базе FileData - хранение информации о файлах
            _DBDataTemplateInDB = scannerData;                // Подключение к базе ScannerDataTemplate - хранение шаблонов
            _Logger = logger;
            _Configuration = configuration;
            _Observer = observer;
            _RabbitMQService = rabbitMqService;

            ScanDocuments = new ObservableCollection<FileData>();

            IndexedDocs = new ObservableCollection<FileData>(_DBFileDataInDB.GetAll().Where(i => i.Indexed && !i.OnRework && !i.Checked));
            var removedItem = new List<FileData>();
            foreach (var indexedDoc in IndexedDocs)
            {
                if(File.Exists(indexedDoc.FilePath))
                    continue;

                _DBFileDataInDB.Delete(indexedDoc.Id);
                removedItem.Add(indexedDoc);
            }

            if (removedItem.Any())
                foreach (var fileData in removedItem)
                    IndexedDocs.Remove(fileData);
            
            GetFiles();
            foreach (var fileData in _DBFileDataInDB.GetAll().Where(i => !i.Checked && i.OnRework))
                ScanDocuments.Add(fileData);

            Templates = new ObservableCollection<ScannerDataTemplate>(_DBDataTemplateInDB.GetAll());

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
                var str = dir.Split('\\');
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
            var testDBWindow = new testDB();
            testDBWindow.ShowDialog();
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

        #region SaveFileCommand - Команда сохранения файла после индексирования

        private ICommand _SaveFileCommand;
        public ICommand SaveFileCommand => _SaveFileCommand
            ??= new LambdaCommand(OnSaveFileCommandExecuted, CanSaveFileCommandExecute);

        private void OnSaveFileCommandExecuted(object p) => SaveFile();

        private bool CanSaveFileCommandExecute(object p)
        {
            if (Metadatas.Count <= 0 || SelectedDocument == null) return false;
            var v = Metadatas.FirstOrDefault(m => string.IsNullOrEmpty(m.Data) || m.Data == " ");
            var result = v == null;

            return result;
        }

        /// <summary>Сохранение файла</summary>
        private void SaveFile()
        {
            var file = SelectedDocument;

            if (file is null) return;

            if (file.Indexed)
            {
                if (file.DocumentName.Contains("(Доработать)"))
                    file.DocumentName = file.DocumentName.Replace("(Доработать)", string.Empty);

                if (file.OnRework)
                    file.OnRework = false;

                file.Document.Metadata.Clear();
                file.Document.DocumentType = SelectedTemplate.DocumentType;
                
                foreach (var data in Metadatas) 
                    file.Document.Metadata.Add(data);

                _DBFileDataInDB.Update(file);
                IndexedDocs.Add(file);
                ScanDocuments.Remove(file);

                SelectedDocument = null;
                Metadatas.Clear();
                FilteredScanDocuments.Remove(file);
                return;
            }
            
            var path = _Configuration["Directories:StorageDirectory"];
            path = Path.GetFullPath(path);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var s = Path.Combine(path, Guid.NewGuid().ToString("N") + ".pdf");

            var oldPath = file.FilePath;
            file.Document.IndexingDate = DateTime.Now;
            file.FilePath = s;
            file.Indexed = true;
            File.Copy(oldPath, s);
                
            foreach (var m in Metadatas)
            {
                file.Document.Metadata.Add(new DocumentMetadata
                {
                    Name = m.Name,
                    Data = m.Data,
                });
            }

            file.Document.DocumentType = SelectedTemplate.DocumentType;
            var fileInDB = _DBFileDataInDB.Add(file);

            IndexedDocs.Add(fileInDB);
            ScanDocuments.Remove(file);
            FilteredScanDocuments.Remove(file);

            SelectedDocument = null;
            Metadatas.Clear();

            IsNew = false;
            Status = "Готов";
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

        #region DeleteExtraMetadataTemplate - Команда удаления поля в редактируемом шаблоне

        private ICommand _DeleteExtraMetadataTemplate;
        public ICommand DeleteExtraMetadataTemplate => _DeleteExtraMetadataTemplate
            ??= new LambdaCommand(OnDeleteExtraMetadataTemplateExecuted, CanDeleteExtraMetadataTemplateExecute);

        private void OnDeleteExtraMetadataTemplateExecuted(object p) => DeleteExtraMetadata(p);
        private bool CanDeleteExtraMetadataTemplateExecute(object p) => true;        

        private void DeleteExtraMetadata(object p)
        {
            if(p is TemplateMetadata tm)
                RowsSelectedEditTemplateAdmin.Remove(tm);
            //RowsSelectedEditTemplateAdmin.Remove(RowSelectedEditTemplateAdmin);
        }

        #endregion

        #region AddExtraDataToDocument - Команда добавления поля Data в редактируемый документ

        private ICommand _AddExtraDataToDocument;
        public ICommand AddExtraDataToDocument => _AddExtraDataToDocument
            ??= new LambdaCommand(OnAddExtraDataToDocumentExecuted, CanAddExtraDataToDocumentExecute);

        private void OnAddExtraDataToDocumentExecuted(object p) => AddDataToDocument();
        private bool CanAddExtraDataToDocumentExecute(object p) => SelectedExtraNoRequiredMetadata is not null;

        private void AddDataToDocument()
        {
            if (SelectedExtraNoRequiredMetadata?.Name != null && SelectedDocument != null)
                Metadatas.Add(new DocumentMetadata
                {
                    Name = SelectedExtraNoRequiredMetadata.Name
                });
        }

        #endregion

        #region DeleteExtraDataFromDocument - Команда удаления элемента метаднных из списка метаданных документа

        private ICommand _DeleteExtraDataFromDocument;
        public ICommand DeleteExtraDataFromDocument => _DeleteExtraDataFromDocument
            ??= new LambdaCommand(OnDeleteExtraDataFromDocumentExecuted, CanDeleteExtraDataFromDocumentExecute);

        private void OnDeleteExtraDataFromDocumentExecuted(object p) => DeleteDataFromDocument(p);
        private bool CanDeleteExtraDataFromDocumentExecute(object p)
        {
            var result = p is null;
            if (p is DocumentMetadata meta)
            {
                var v = SelectedTemplate?.TemplateMetadata.FirstOrDefault(t => t.Name == meta.Name);
                if (v == null)
                    return false;
                if (!v.Required)
                    result = true;
            }

            return result;
        }

        private void DeleteDataFromDocument(object p)
        {
            if (p is DocumentMetadata meta) Metadatas.Remove(meta);
        }

        #endregion

        #region SaveEditTemplateToBD - команда сохранения шаблона в базу

        private ICommand _SaveEditTemplateToBD;
        public ICommand SaveEditTemplateToBD => _SaveEditTemplateToBD
            ??= new LambdaCommand(OnSaveEditTemplateToBDExecuted, CanSaveEditTemplateToBDExecute);

        private void OnSaveEditTemplateToBDExecuted(object p) => SaveEditTemplate();
        private bool CanSaveEditTemplateToBDExecute(object p) => true;

        private void SaveEditTemplate()
        {
            var dataTemplate = SelectedEditTemplateAdmin;
            dataTemplate.TemplateMetadata = RowsSelectedEditTemplateAdmin;

            if (Templates.Contains(dataTemplate))
                _DBDataTemplateInDB.Update(dataTemplate);
            else
            {
                if (Templates.FirstOrDefault(t => t.DocumentType == dataTemplate.DocumentType) == null)
                {
                    _DBDataTemplateInDB.Add(dataTemplate);
                    Templates.Add(dataTemplate);
                }
                else
                    MessageBox.Show("Имя шаблона должно быть уникальным!!!");
            }
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
            if (SelectedEditTemplateAdmin == null) return;
            if (MessageBox.Show($"Вы уверены что хотите удалить {SelectedEditTemplateAdmin.DocumentType}?", "Удалить",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _DBDataTemplateInDB.Delete(SelectedEditTemplateAdmin.Id);
                Templates.Remove(SelectedEditTemplateAdmin);
            }
        }

        #endregion

        #region CreateNewTemplate - команда создания нового шаблона

        private ICommand _CreateNewTemplate;
        public ICommand CreateNewTemplate => _CreateNewTemplate
            ??= new LambdaCommand(OnCreateNewTemplateExecuted, CanCreateNewTemplateExecute);
        private void OnCreateNewTemplateExecuted(object p) => CreateTemplate();
        private bool CanCreateNewTemplateExecute(object p) => true;

        private void CreateTemplate()
        {
            ScannerDataTemplate template = new ScannerDataTemplate { DocumentType = "Новый шаблон", };
            SelectedEditTemplateAdmin = template;
        }

        #endregion

        #region AdminFinishCommand - Команда завершения обработки

        private ICommand _AdminFinishCommand;

        public ICommand AdminFinishCommand => _AdminFinishCommand
            ??= new LambdaCommand(OnAdminFinishCommandExecuted, CanAdminFinishCommandExecute);

        private void OnAdminFinishCommandExecuted(object p)
        {
            if (SelectedIndexedDoc is null) return;
            _RabbitMQService.PublishAsync(SelectedIndexedDoc, SelectedTemplate.Id);

            var doc = SelectedIndexedDoc;
            doc.Checked = true;

            _DBFileDataInDB.Update(doc);
            IndexedDocs.Remove(doc);
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
            doc.DocumentName = "(Доработать)" + doc.DocumentName;
            doc.OnRework = true;
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