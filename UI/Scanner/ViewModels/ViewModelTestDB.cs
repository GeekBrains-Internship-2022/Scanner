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

        private ScannerDataTemplate _SelectedTemplate;

        public ScannerDataTemplate SelectedTemplate
        {
            get { return _SelectedTemplate; }
            set { Set(ref _SelectedTemplate, value); }
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
            //FileData fd = new FileData();
            //fd.FilePath = Path;
            //fd.Document = new Document { DocumentType = Type };
            //FileData.Add(_filedata.Add(fd));
            var fd = _filedata.Add(new FileData());
            fd.Document = new Document();
            fd.FilePath = Path;
            fd.Document.DocumentType = Type;
            List<DocumentMetadata> meta = new List<DocumentMetadata>();
            meta.Add(new DocumentMetadata());
            fd.Document.Metadata = meta.ToArray();
            _filedata.Update(fd);
            FileData.Add(fd);
        }
        #endregion

        #region Комманда для добавления метаданных
        private ICommand _AddTemplateMetadata;

        public ICommand AddTemplateMetadata => _AddTemplateMetadata
            ??= new LambdaCommand(OnAddTemplateMetadata, CanAddTemplateMetadata);

        private bool CanAddTemplateMetadata(object p)
        {
            if (SelectedTemplate != null) return true;
            else return false;
        }

        private void OnAddTemplateMetadata(object p)
        {
            if (SelectedTemplate == null) return;
            var value = (object[])p;
            var Name = (string)value[0];
            var Required = (bool)value[1];
            SelectedTemplate.TemplateMetadata?.Add(new TemplateMetadata { ScannerDataTemplate = SelectedTemplate,
                Name = Name, Required = Required });
            //FileData fd = new FileData();
            //fd.FilePath = Path;
            //fd.Document = new Document { DocumentType = Type };
            //FileData.Add(_filedata.Add(fd));
            
        }
        #endregion

        #region Комманда для добавления метаданных
        private ICommand _CreateNewTemplate;

        public ICommand CreateNewTemplate => _CreateNewTemplate
            ??= new LambdaCommand(OnCreateNewTemplate, CanCreateNewTemplate);

        private bool CanCreateNewTemplate(object p) => true;

        private void OnCreateNewTemplate(object p)
        {
            SelectedTemplate = new ScannerDataTemplate();
        }
        #endregion

        #region Комманда для Сохранения шаблона
        private ICommand _SaveTemplateDocument;

        public ICommand SaveTemplateDocument => _SaveTemplateDocument
            ??= new LambdaCommand(OnSaveTemplateDocument, CanSaveTemplateDocument);

        private bool CanSaveTemplateDocument(object p) { 
        
            if (SelectedTemplate == null) return false;
            else return true;
        }

        private void OnSaveTemplateDocument(object p)
        {
         if (SelectedTemplate.DocumentType != null)
            {
                _scannerData.Update(SelectedTemplate);
            }         
         else
            {                
                var name = (string)p;
                SelectedTemplate.DocumentType = name;                
                ScannerDataTemplates.Add(_scannerData.Add(SelectedTemplate));
            }
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
