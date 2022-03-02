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
using Newtonsoft.Json.Linq;

namespace Scanner.ViewModels
{
    class SettingsWindowViewModel : ViewModel
    {
        public SettingsModel Settings { get; set; }
        public ObservableCollection<string> PropertyInfo { get; set; } = new();

        public SettingsWindowViewModel()
        {
            ObservableCollection<string> roots = new ObservableCollection<string>();
            Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();            

            string jsonString = File.ReadAllText("appsettings.json");
            dynamic data = JObject.Parse(jsonString).Children();
            int i = 0;
            foreach (var c in data)
            {
                string[] str = c.ToString().Split(":");                
                roots.Add(str[0].Trim(new Char[] { '\\', '"' }));                
                List<string> list = new List<string>();
                
                foreach (var v in c)
                {
                    if (!values.ContainsKey(roots[i]))
                    {
                        string[] t = v.ToString().Split(",");
                        values.Add(roots[i], t.ToList());
                    }
                    else
                        values[roots[i]].Add(v.ToString());
                    i++;
                }
            }
        }
        
    }
}
