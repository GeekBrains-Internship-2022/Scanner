using Scanner.ViewModels.Base;
using Scanner.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Scanner.ViewModels
{
    class SettingsWindowViewModel : ViewModel
    {
        public SettingsModel Settings { get; set; }
        public ObservableCollection<string> PropertyInfo { get; set; } = new();

        public SettingsWindowViewModel()
        {            
            Type[] types;

            Type myType = typeof(SettingsModel);

            types = myType.GetNestedTypes();

            foreach (var t in types)
            {
                PropertyInfo[] myPropertyInfo;
                myPropertyInfo = t.GetProperties();

                foreach (var prop in myPropertyInfo)
                    PropertyInfo.Add(prop.Name);
            }


            //foreach (var prop in myPropertyInfo)
            //    PropertyInfo.Add(prop.ToString());

            string jsonString = File.ReadAllText("appsettings.json");
            Settings = JsonSerializer.Deserialize<SettingsModel>(jsonString);
        }
        
    }
}
