using Microsoft.Extensions.Logging;
using Scanner.Data.Stores.InDB;
using Scanner.Infrastructure.Commands;
using Scanner.interfaces;
using Scanner.interfaces.RabbitMQ;
using Scanner.Models;
using Scanner.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

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


        #region Поля и свойства для отображения на вкладке Оператор
        private ObservableCollection<DocumentMetadata> _documentMetadata;
        /// <summary>
        /// Коллекция метаданных выбранного файла для отображения в окне оператора
        /// </summary>
        public ObservableCollection<DocumentMetadata> DocumentMetadataInOP
        {
            get { return _documentMetadata; }
            set { Set(ref _documentMetadata, value); }
        }

        #region Колекция доступных полей при заполнение карточки метаданных
        private ObservableCollection<TemplateMetadata> _templateMetadatas;

        public ObservableCollection<TemplateMetadata> TemplateMetadatas
        {
            get { return _templateMetadatas; }
            set { Set(ref _templateMetadatas, value); }
        }
        #endregion

        /// <summary>
        /// Коллекция для отображения файлов на вкладке Оператор с возможностью фильтрации
        /// </summary>
        public ICollectionView FileDatasInListOP { get; }

        #region Выбранный элемент в ComboBox на панеле оператора для фильтрации списка файлов
        private string _selectedFIlterInGroupboxOP;

        /// <summary>
        /// Выбранный элемент в ComboBox на панеле оператора для фильтрации списка файлов
        /// </summary>
        public string SelectedFIlterInGroupboxOP
        {
            get { return _selectedFIlterInGroupboxOP; }
            set
            {
                Set(ref _selectedFIlterInGroupboxOP, value);
                FileDatasInListOP.Refresh();
            }
        }
        #endregion

        #region Выбранный элемент в панеле оператора, из списка файлов
        private FileData _SelectedFileDataInOperatorPanel;

        /// <summary>
        /// Выбранный элемент в панеле оператора, из списка файлов
        /// </summary>
        public FileData SelectedFileDataInOperatorPanel
        {
            get { return _SelectedFileDataInOperatorPanel; }
            set
            {
                Set(ref _SelectedFileDataInOperatorPanel, value);

                if (value.Document?.DocumentType != null)
                {
                    var temp = ScannerDataTemplates.FirstOrDefault(o => o.DocumentType == value.Document?.DocumentType);
                    if (temp != null) SelectedTemplateInOP = temp;
                    //else SelectedTemplateInOP = new ScannerDataTemplate { DocumentType= value.Document?.DocumentType };
                }              
            }
        }
        #endregion

        #region Выбранный шаблон в окне оператора
        private ScannerDataTemplate _selectedTemplateInOP;

        /// <summary>
        /// Выбранный шаблон в окне оператора
        /// </summary>
        public ScannerDataTemplate SelectedTemplateInOP
        {
            get { return _selectedTemplateInOP; }
            set
            {
                ClearCollectionMetadata();
                UpdateDocumentMetadataInOP(value);
                Set(ref _selectedTemplateInOP, value);
            }
        }
        #endregion
        #endregion

        private ObservableCollection<TemplateMetadata> _TemplateMetadataInViewAdm;
        /// <summary>
        /// Отображение метаданных выбранного шаблона
        /// </summary>
        public ObservableCollection<TemplateMetadata> TemplateMetadataInView
        {
            get { return _TemplateMetadataInViewAdm; }
            set { Set(ref _TemplateMetadataInViewAdm, value); }
        }

        private ScannerDataTemplate _SelectedTemplateInView;
        /// <summary>
        /// Выбранный шаблон для редактирования метданных документа
        /// </summary>
        public ScannerDataTemplate SelectedTemplateInView
        {
            get { return _SelectedTemplateInView; }
            set { 
                Set(ref _SelectedTemplateInView, value);
                TemplateMetadataInView.Clear();
                UpdateTemplateMetadataInView();
            }
        }
        
        private TemplateMetadata _SelectedTemplateMetadate;
        /// <summary>
        /// Поле содержащие в себе выбранные метедаданные для добалвения в колекцию метданных документа
        /// </summary>
        public TemplateMetadata SelectedTemplateMetadate
        {
            get => _SelectedTemplateMetadate;
            set => Set(ref _SelectedTemplateMetadate, value);

        }
        #region Поле для сортировки шаблонов на вкладке шаблоны
        private string _SearchTemplate;
        /// <summary>
        /// Поле для сортировки шаблонов на вкладке шаблоны
        /// </summary>
        public string SearchTemplate
        {
            get { return _SearchTemplate; }
            set { 
                Set(ref _SearchTemplate, value);
                TemplateCollectionViewer.Refresh();
            }
        }
        #endregion
        /// <summary>
        /// Коллекция для отображение шаблонов
        /// </summary>
        public ICollectionView TemplateCollectionViewer { get; }

        /// <summary>
        /// Хренотень, сообщающая какая вкладка отображается, для применения фильтров к списку файлов
        /// </summary>
        private bool _IsSelectedPeerreview;

        public bool IsSelectedPeerreview
        {
            get { return _IsSelectedPeerreview; }
            set { 
                Set(ref _IsSelectedPeerreview, value);
                FileDatasInListOP.Refresh();
            }
        }

        //Not processed Не обработанные
        
        private bool _InProcessedFileDataFilter;

        public bool InProcessedFileDataFilter
        {
            get { return _InProcessedFileDataFilter; }
            set
            {
                Set(ref _InProcessedFileDataFilter, value);
                FileDatasInListOP.Refresh();
            }
        }
        //Completed Завершенные

        private bool _InCompletedFileDataFilter;

        public bool InCompletedFileDataFilter
        {
            get { return _InCompletedFileDataFilter; }
            set
            {
                Set(ref _InCompletedFileDataFilter, value);
                FileDatasInListOP.Refresh();
            }
        }

        #region Команды


        #region Команда удаления элемента метаднных из списка метаданных документа
        /// <summary>
        /// Команда удаления элемента метаднных из списка метаданных документа
        /// </summary>
        public ICommand DelleteMetaDataInDocumentViewOP { get; }

        private void OnDelleteMetaDataInDocumentViewOP(object p)
        {

            if (p is DocumentMetadata meta)
            {
                DocumentMetadataInOP.Remove(meta);
            }
            return;
        }

        private bool CanDelleteMetaDataInDocumentViewOP(object p)
        {

            if (p is DocumentMetadata meta)
            {
                var temp = SelectedTemplateInOP.TemplateMetadata.FirstOrDefault(o => o.Name == meta.Name);
                if (temp == null) return true;
                if (temp.Required) return false;
            }
            //
            return true;
        }
        #endregion

        public ICommand AddMetaDataInDocumentViewOP { get; }

        private void OnAddMetaDataInDocumentViewOP(object p)
        {
            //SelectedTemplateMetadate
            //TemplateMetadatas
            //DocumentMetadataInOP
            DocumentMetadataInOP.Add(new DocumentMetadata { Name = SelectedTemplateMetadate.Name });
            TemplateMetadatas.Remove(SelectedTemplateMetadate);            
        }

        private bool CanAddMetaDataInDocumentViewOP(object p)
        {

            if (SelectedTemplateMetadate is null) return false;
            return true;
        }


        #endregion
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
            DocumentMetadataInOP = new ObservableCollection<DocumentMetadata>();
            TemplateMetadatas = new ObservableCollection<TemplateMetadata>();

            FileDatasInListOP = CollectionViewSource.GetDefaultView(FileDatas);

            FileDatasInListOP.Filter = FilterFileDatasInListOP;

            TemplateCollectionViewer = CollectionViewSource.GetDefaultView(ScannerDataTemplates);

            TemplateCollectionViewer.Filter = FilterTemplateCollectionViewer;

            TemplateMetadataInView = new ObservableCollection<TemplateMetadata>();


            DelleteMetaDataInDocumentViewOP = new LambdaCommand(OnDelleteMetaDataInDocumentViewOP, CanDelleteMetaDataInDocumentViewOP);

            AddMetaDataInDocumentViewOP = new LambdaCommand(OnAddMetaDataInDocumentViewOP, CanAddMetaDataInDocumentViewOP);

            //ObserverInitialize();
#if DEBUG
            TestDataInit();
#endif
        }

        private bool FilterTemplateCollectionViewer(object obj)
        {
            if (obj is ScannerDataTemplate scannerDataTemplate)
            {
                if (SearchTemplate is null || SearchTemplate == "") return true;
                var contains = scannerDataTemplate.DocumentType.Contains(SearchTemplate);
                return contains;
            }
            return false;
        }

        private void UpdateTemplateMetadataInView()
        {
            if (SelectedTemplateInView.TemplateMetadata is null) return;
            foreach (var item in SelectedTemplateInView.TemplateMetadata)
            {
                TemplateMetadataInView.Add(item);
            }
        }

        #region Методы для отображения на вкладке Оператор
        private void ClearCollectionMetadata()
        {
            DocumentMetadataInOP.Clear();
            TemplateMetadatas.Clear();
        }
        private void UpdateDocumentMetadataInOP(ScannerDataTemplate value)
        {
            if (value is null) return;
            //var template = ScannerDataTemplates.FirstOrDefault(o => o.DocumentType == value);
            if (value.TemplateMetadata == null || SelectedFileDataInOperatorPanel == null) return;

            var meta = SelectedFileDataInOperatorPanel.Document.Metadata?.ToArray();
            List<DocumentMetadata> list;
            if (meta != null) list = new List<DocumentMetadata>(meta);
            else list = new List<DocumentMetadata>();

            foreach (var item in value.TemplateMetadata)
            {
                if (item.Required)
                {
                    DocumentMetadata data = list.FirstOrDefault(o => o.Name == item.Name);
                    DocumentMetadataInOP.Add(new DocumentMetadata { Name = item.Name, Data = data?.Data });
                    if (data != null) list.Remove(data);
                }
                else
                {
                    DocumentMetadata data = list.FirstOrDefault(o => o.Name == item.Name);
                    if (data == null) TemplateMetadatas.Add(item);
                    else
                    {
                        DocumentMetadataInOP.Add(new DocumentMetadata { Name = item.Name, Data = data?.Data });
                    }
                }
            }
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    DocumentMetadataInOP.Add(new DocumentMetadata { Name = item.Name, Data = item.Data });
                }
            }
        }

        private bool FilterFileDatasInListOP(object obj)
        {
            if (obj is FileData filedata)
            {
                if (IsSelectedPeerreview)
                {
                    if (filedata.Indexed == !InProcessedFileDataFilter && filedata.Checked == InCompletedFileDataFilter)
                    {
                        if (SelectedFIlterInGroupboxOP is null || SelectedFIlterInGroupboxOP == "") return true;
                        var contains = filedata.Document.DocumentType.Contains(SelectedFIlterInGroupboxOP);
                        return contains;
                    }                                           
                }
                else
                {
                    if (!filedata.Indexed && !filedata.Checked)
                    {
                        if (SelectedFIlterInGroupboxOP is null || SelectedFIlterInGroupboxOP == "") return true;
                        var contains = filedata.Document.DocumentType.Contains(SelectedFIlterInGroupboxOP);
                        return contains;
                    }
                }
                
            }
            return false;
        } 
        #endregion

        private void TestDataInit()
        {
            List<FileData> fileDatas = new List<FileData>();
            List<ScannerDataTemplate> scannerDataTemplates = new List<ScannerDataTemplate>();

            var doc = new Document {
                DocumentType = "Паспорт",
                IndexingDate = DateTime.Now,
            };
            var meta1 = new DocumentMetadata
            {
                Document = doc,
                Name = "ФИО",
                Data = "Пупкин Василий Петрович"
            };
            var meta2 = new DocumentMetadata
            {
                Document = doc,
                Name = "Выдан",
                Data = "01.01.0000",
            };
            doc.Metadata = new List<DocumentMetadata> { meta1,meta2}.ToArray();
            //doc.Metadata.Add(meta1);
            //doc.Metadata.Add(meta2);


            fileDatas.Add(new FileData
            {
                 DateAdded = DateTime.Now,
                  DocumentName = "Паспорт васи",
                   FilePath = "D:\\github.com\\GeekBrains-Internship-2022\\Scanner\\UI\\Scanner\\bin\\Debug\\net5.0-windows\\Documents\\Диплом\\Диплом1.pdf",
                    Document = doc,
                     Indexed=false,
            });
            fileDatas.Add(new FileData
            {
                DateAdded = DateTime.Now,
                DocumentName = "scan0004",
                FilePath = "c:\\Паспорт\\scan0004.pdf",
                Document = new Document
                {
                    DocumentType = "Паспорт"                    
                },
                Indexed = false,
            });
            fileDatas.Add(new FileData
            {
                DateAdded = DateTime.Now,
                DocumentName = "scan00001",
                FilePath = "c:\\Договор\\scan00001pdf",
                Document = new Document
                {
                    DocumentType = "Договор",
                    IndexingDate = DateTime.Now,                    
                },
                Indexed = false,
            });
            fileDatas.Add(new FileData
            {
                DateAdded = DateTime.Now,
                DocumentName = "Сертификат",
                FilePath = "c:\\Сертификат\\Сертификат.pdf",
                Document = new Document
                {
                    DocumentType = "Сертификат",
                },
                Indexed = false,
            });

            scannerDataTemplates.Add(new ScannerDataTemplate { 
             DocumentType= "Паспорт",
              TemplateMetadata = { new TemplateMetadata {
                 Name="ФИО",
                  Required = true,
              }, new TemplateMetadata {
                 Name="Кем выдан",
                  Required = false,
              },new TemplateMetadata {
                 Name="Когда выдан",
                  Required = false,
              },

                }
            });

            foreach (var sd in scannerDataTemplates)
            {
                ScannerDataTemplates.Add(sd);
            }
            foreach( var fd in fileDatas)
            {
                FileDatas.Add(fd);
            }
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
