using Scanner.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner.ViewModels.Models
{
    public class Metadata : OnPropertyChangedClass
    {
        private string _name;

        public string Name { get => _name; set => _name = value; }

        #region Data : string - метаданные

        private string _Data;

        public string Data
        {
            get => _Data;
            set => Set(ref _Data, value);
        }

        #endregion
    }
}
