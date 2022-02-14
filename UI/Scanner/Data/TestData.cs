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
        private static Random _Rand = new();
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
                    Metadata = new ObservableCollection<Metadata>
                    {
                        new Metadata
                        {
                            Name = "Номер",
                            Data = "1234567890",
                            Required = true,
                        },
                        new Metadata
                        {
                            Name = "ФИО",
                            Data = "Иванов Иван Иванович",
                            Required = true,
                        },
                        new Metadata
                        {
                            Name = "Дата выдачи",
                            Data = "01.01.0001",
                            Required = true,
                        },
                        new Metadata
                        {
                            Name = "Прописка",
                            Data = "Кремль",
                            Required = true,
                        },
                    }
                },
                new Template()
                {
                    Name = "Свидетельство",
                    Metadata = new ObservableCollection<Metadata>
                    {
                        new Metadata
                        {
                            Name = "Номер",
                            Data = "1234567890",
                            Required = true,
                        },
                        new Metadata
                        {
                            Name = "ФИО",
                            Data = "Петров Петр Петрович",
                            Required = true,
                        },
                        new Metadata
                        {
                            Name = "Дата выдачи",
                            Data = "01.01.0001",
                            Required = true,
                        },
                        new Metadata
                        {
                            Name = "Дата рождения",
                            Data = "1000 г. до н.э.",
                            Required = true,
                        },
                        new Metadata
                        {
                            Name = "ФИО отца",
                            Data = "Петров Петро Петрович",
                            Required = true,
                        },
                        new Metadata
                        {
                            Name = "ФИО матери",
                            Data = "Александрова Александра Александровна",
                            Required = true,
                        },
                    }
                },
            };

            Documents = new ObservableCollection<Document>(Enumerable.Range(1, 10).Select(d =>
                new Document
                {
                    Id = d,
                    DocumentType = $"DocType {d}",
                    IndexingDate = new DateTime(_Rand.Next(2000, 2021), _Rand.Next(1, 12), _Rand.Next(1, 20)),
                    Metadata = Enumerable.Range(1, 10).Select(m => new DocumentMetadata
                    {
                        Id = m,
                        Name = $"Name {m}",
                        Data = $"Data {m}"
                    }
                    ).ToList()
                }));

            Files = new ObservableCollection<FileData>();
        }
    }
}
