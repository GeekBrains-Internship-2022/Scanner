using Scanner.Models.Base;
using Scanner.ViewModels.Models;
using System.Collections.ObjectModel;

namespace Scanner.Models
{
    public class Template : OnPropertyChangedClass
    {
        private string _name;        
        private ObservableCollection<TemplateMetadata> _templateMetadata;

        public string Name { get => _name; set => _name = value; }

        public ObservableCollection<TemplateMetadata> TemplateMetadata { get => _templateMetadata; set => _templateMetadata = value; }        
    }
}
