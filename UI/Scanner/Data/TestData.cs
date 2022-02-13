using Scanner.Models;
using Scanner.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner.Data
{
    internal class TestData
    {
        public ObservableCollection<Template> Templates { get; set; }

        public ObservableCollection<Document> Documents { get; set; }

        public ObservableCollection<FileData> Files { get; set; }

        public TestData()
        {
            Templates = new ObservableCollection<Template>
            {
                new Template()
                {
                    Name = "Паспорт",
                    Metadata =  new ObservableCollection<Metadata>
                    {
                        new Metadata
                        {
                            Name ="Номер",
                            Required = true,
                        },
                        new Metadata
                        {
                            Name ="ФИО",
                            Required = true,
                        },
                        new Metadata
                        {
                            Name ="Дата выдачи",
                            Required = true,
                        },
                        new Metadata
                        {
                            Name ="Прописка",
                            Required = true,
                        },
                    }
                },
                new Template()
                {
                    Name = "Свидетельство",
                    Metadata =  new ObservableCollection<Metadata>
                    {
                        new Metadata
                        {
                            Name ="Номер",
                            Required = true,
                        },
                        new Metadata
                        {
                            Name ="ФИО",
                            Required = true,
                        },
                        new Metadata
                        {
                            Name ="Дата выдачи",
                            Required = true,
                        },
                        new Metadata
                        {
                            Name ="Дата рождения",
                            Required = true,
                        },
                        new Metadata
                        {
                            Name ="ФИО отца",
                            Required = true,
                        },
                        new Metadata
                        {
                            Name ="ФИО матери",
                            Required = true,
                        },
                    }
                },
            };
            
            Documents = new ObservableCollection<Document>();

            Files = new ObservableCollection<FileData>();
        }
    }
}
