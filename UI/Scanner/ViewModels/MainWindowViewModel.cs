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

        public IList<string> ScaningDocuments { get; } = new ObservableCollection<string>(); //Список отсканированных файлов

        public string Title { get => _title; set => Set(ref _title, value); }

        public MainWindowViewModel()
        {
            ScaningDocuments.Add("паспорт.pdf");
        }
    }
}
