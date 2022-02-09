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
        private string _title = "Сканировщик";

        public IList<ScanDocument> ScaningDocuments { get; } = new ObservableCollection<ScanDocument>(); //Список отсканированных файлов
        public IList<DocumetnFilter> DocumetnFilters { get; } = new ObservableCollection<DocumetnFilter>(); //Список фильтров

        public string Title { get => _title; set => Set(ref _title, value); }

        public MainWindowViewModel()
        {
            ScaningDocuments.Add(new ScanDocument { Date = DateTime.Today, FilePath = "", Name = "1", Type = "Паспорт" });
            ScaningDocuments.Add(new ScanDocument { Date = DateTime.Today, FilePath = "", Name = "2", Type = "Диплом" });

            DocumetnFilters.Add(new DocumetnFilter { FilterName = "По имени" });
            DocumetnFilters.Add(new DocumetnFilter { FilterName = "По типу" });
            DocumetnFilters.Add(new DocumetnFilter { FilterName = "По времени" });
        }

        private void DeleteScanDocument(ScanDocument document)
        {
            ScaningDocuments.Remove(document);
        }

        public class ScanDocument
        {
            public string FilePath { get; set; }

            public DateTime Date { get; set; }

            public string Type { get; set; }

            public string Name { get; set; }
        }

        public class DocumetnFilter
        {
            public string FilterName { get; set; }
        }
    }
}
