using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scanner.interfaces;
using Scanner.Service;
using Scanner.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner.ViewModels
{
    class MainWindowViewModel: ViewModel
    {
        private static readonly IConfiguration __Configuration = App.Services.GetRequiredService<IConfiguration>();
        private readonly IFileService fileService;
        private readonly ObserverService observerService;
        private readonly string path = __Configuration["ObserverPath"];
        private List<string> _types = new List<string>();
        //private List<string[]> _filesList = new List<string[]>();
        private string _title = "Сканировщик";

        public IList<ScanDocument> ScaningDocuments { get; } = new ObservableCollection<ScanDocument>(); //Список отсканированных файлов
        public IList<DocumetnFilter> DocumetnFilters { get; } = new ObservableCollection<DocumetnFilter>(); //Список фильтров
        public ScanDocument SelectDocument;

        public string Title { get => _title; set => Set(ref _title, value); }

        public MainWindowViewModel(IFileService fileService, ObserverService observerService)
        {
            ScaningDocuments = GetScanDocuments();

            DocumetnFilters.Add(new DocumetnFilter { FilterName = "По имени" });
            DocumetnFilters.Add(new DocumetnFilter { FilterName = "По типу" });
            DocumetnFilters.Add(new DocumetnFilter { FilterName = "По времени" });
            this.fileService = fileService;
            this.observerService = observerService;
            this.observerService.Notify += ObserverService_Notify;
            //this.observerService.Start(path);
        }

        private void ObserverService_Notify(string message)
        {
            var documenttype = message.Substring((path + "\\").Length);
            //fileService.CreateFileData(message, documenttype);
        }

        private void DeleteScanDocument(ScanDocument document)
        {
            ScaningDocuments.Remove(document);
        }

        private ObservableCollection<ScanDocument> GetScanDocuments()
        {
            var path = __Configuration["ObserverPath"];

            ObservableCollection<ScanDocument> documents = new ObservableCollection<ScanDocument>();

            string[] subDirs = null;            

            try
            {
                subDirs = System.IO.Directory.GetDirectories(path);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (subDirs != null)
            {
                foreach (var dir in subDirs)
                {
                    List<string[]> _filesList = new List<string[]>();
                    try
                    {
                        _filesList.Add(System.IO.Directory.GetFiles(dir));
                        var s = dir.Substring((path + "\\").Length);    //получаем тип из названия папки
                        _types.Add(s);

                        foreach (var files in _filesList.ToArray())
                        {
                            foreach (var file in files)
                            {
                                System.IO.FileInfo fileInfo = new System.IO.FileInfo(file);
                                documents.Add(new ScanDocument { Date = fileInfo.CreationTime.ToShortTimeString(), FilePath = file, Name = fileInfo.Name, Type = s });
                            }
                        }
                    }

                    catch (UnauthorizedAccessException e)
                    {

                        Console.WriteLine(e.Message);
                    }

                    catch (System.IO.DirectoryNotFoundException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }     
            }        

            return documents;
        }

        //Класс отсканированного документа
        public class ScanDocument
        {
            public string FilePath { get; set; }

            public string Date { get; set; }

            public string Type { get; set; }

            public string Name { get; set; }
        }

        //Класс фильтра
        public class DocumetnFilter
        {
            public string FilterName { get; set; }
        }
    }
}
