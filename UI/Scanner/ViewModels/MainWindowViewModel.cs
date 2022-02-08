using Scanner.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner.ViewModels
{
    class MainWindowViewModel: ViewModel
    {
        private string _title = "Сканировщик";

        public string Title { get => _title; set => Set(ref _title, value); }

        public MainWindowViewModel()
        {

        }
    }
}
