using Scanner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Scanner.Infrastructure.Converters
{
    internal class OperatorComboBoxFilterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<ScannerDataTemplate> ovalue)
            {
                var list = new List<string>();
                list.Add("");
                foreach (ScannerDataTemplate file in ovalue)
                {
                    if (!list.Contains(file.DocumentType))
                    {
                        list.Add(file.DocumentType);
                    }
                }
                return list;
            }
            else return null;            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
