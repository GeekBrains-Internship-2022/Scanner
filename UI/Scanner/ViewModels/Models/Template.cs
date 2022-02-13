using Scanner.Models.Base;
using System.Collections.ObjectModel;

namespace Scanner.Models
{
    internal class Template : OnPropertyChangedClass
    {
        private string _name;        
        private ObservableCollection<TemplateMetadata> _metadata;

        public string Name { get => _name; set => _name = value; }

        public ObservableCollection<TemplateMetadata> Metadata { get => _metadata; set => _metadata = value; }        
    }
}
