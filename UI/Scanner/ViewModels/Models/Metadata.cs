using Scanner.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner.ViewModels.Models
{
    internal class Metadata : OnPropertyChangedClass
    {
        private string _name;
        private bool _required;

        public string Name { get => _name; set => _name = value; }

        public bool Required { get => _required; set => _required = value; }
    }
}
