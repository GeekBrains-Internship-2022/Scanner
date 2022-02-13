using Scanner.Models.Base;
using Scanner.ViewModels.Models;
using System.Collections.ObjectModel;

namespace Scanner.Models
{
    internal class Template : OnPropertyChangedClass
    {
        private string _name;        
        private ObservableCollection<Metadata> _metadata;

        public string Name { get => _name; set => _name = value; }

        public ObservableCollection<Metadata> Metadata { get => _metadata; set => _metadata = value; }        
    }
}
