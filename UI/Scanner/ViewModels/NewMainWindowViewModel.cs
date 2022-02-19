using Microsoft.Extensions.Logging;
using Scanner.Data.Stores.InDB;
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
        public ICollectionView FileDatasInListOP { get;}

        #region Выбранный элемент в ComboBox на панеле оператора для фильтрации списка файлов
        private string _selectedFIlterInGroupboxOP;

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

        public FileData SelectedFileDataInOperatorPanel
        {
            get { return _SelectedFileDataInOperatorPanel; }
            set { 
                Set(ref _SelectedFileDataInOperatorPanel, value);                
            }
        }
        #endregion

        #region Выбранный шаблон в окне оператора
        private string _selectedTemplateInOP;

        public string SelectedTemplateInOP
        {
            get { return _selectedTemplateInOP; }
            set {
                ClearCollectionMetadata();
                UpdateDocumentMetadataInOP(value);
                Set(ref _selectedTemplateInOP, value);                
            }
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

            //ObserverInitialize();
#if DEBUG
            TestDataInit();
#endif
        }

        private void ClearCollectionMetadata()
        {
            DocumentMetadataInOP.Clear();
            TemplateMetadatas.Clear();            
        }
        private void UpdateDocumentMetadataInOP(string value)
        {
            if (value is null) return;
            var template = ScannerDataTemplates.FirstOrDefault(o => o.DocumentType == value);
            if (template == null || template.TemplateMetadata == null) return;

            var meta = SelectedFileDataInOperatorPanel.Document.Metadata?.ToArray();
            List<DocumentMetadata> list;
            if (meta != null) list = new List<DocumentMetadata>(meta);
            else list = new List<DocumentMetadata>();

            foreach (var item in template.TemplateMetadata)
            {
                if (item.Required)
                {
                    DocumentMetadata data = list.FirstOrDefault(o => o.Name == item.Name);
                    DocumentMetadataInOP.Add(new DocumentMetadata { Name = item.Name, Data = data?.Data});
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
                foreach(var item in list)
                {
                    DocumentMetadataInOP.Add(new DocumentMetadata { Name = item.Name, Data = item.Data });
                }
            }
        }

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
                   FilePath = "c:\\\\Паспорт\\Паспорт васи.pdf",
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

        private bool FilterFileDatasInListOP(object obj)
        {
            

            if (obj is FileData filedata)
            {
                if (!filedata.Indexed)
                {
                    if (SelectedFIlterInGroupboxOP is null || SelectedFIlterInGroupboxOP == "") return true;
                    var contains = filedata.Document.DocumentType.Contains(SelectedFIlterInGroupboxOP);                    
                    return contains;                    
                }                
            }            
            return false;
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
