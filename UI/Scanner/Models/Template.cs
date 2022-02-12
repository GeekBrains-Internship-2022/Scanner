using Scanner.Models.Base;
using System.Collections.ObjectModel;

namespace Scanner.Models
{
    internal class Template : OnPropertyChangedClass
    {
        private string _name;
        private bool _required;
        private ObservableCollection<string> _metadata;

        public string Name { get => _name; set => _name = value; }

        public ObservableCollection<string> Metadata { get => _metadata; set => _metadata = value; }

        public bool Required { get => _required; set => _required = value; }
    }
}
