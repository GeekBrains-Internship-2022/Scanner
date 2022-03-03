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
        
        /// <summary>
        /// Тестовый список шаблонов
        /// </summary>
        public ObservableCollection<ScannerDataTemplate> DataTemplates { get; set; }

        /// <summary>
        ///  Тестовый список метаданных документов
        /// </summary>
        public ObservableCollection<DocumentMetadata> DocumentMetadatas { get; set; }

        /// <summary>
        /// Тестовый список файлов
        /// </summary>
        public ObservableCollection<FileData> FilesDatas { get; set; }

        /// <summary>
        /// Тестовый список наменований полей в шаблонах
        /// </summary>
        public ObservableCollection<TemplateMetadata> TemplateMetadatas { get; set; }

        public TestData()
        {
            DataTemplates = new ObservableCollection<ScannerDataTemplate>();
            DocumentMetadatas = new ObservableCollection<DocumentMetadata>();
            FilesDatas = new ObservableCollection<FileData>();
            TemplateMetadatas = new ObservableCollection<TemplateMetadata>();

            #region Тестовые данные - шаблоны
            ScannerDataTemplate testScannerDataTemplates = new ScannerDataTemplate();
            List<TemplateMetadata> templateMetadatas = new List<TemplateMetadata>();
            templateMetadatas.Add(new TemplateMetadata
            {
                Name = "Серия",
                Required = true,
            });
            templateMetadatas.Add(new TemplateMetadata
            {
                Name = "Номер",
                Required = true,
            });
            templateMetadatas.Add(new TemplateMetadata
            {
                Name = "ФИО",
                Required = true,
            });
            templateMetadatas.Add(new TemplateMetadata
            {
                Name = "Дата выдачи",
                Required = false,
            });
            templateMetadatas.Add(new TemplateMetadata
            {
                Name = "Кем выдан",
                Required = false,
            });

            testScannerDataTemplates = new ScannerDataTemplate
            {
                DocumentType = "Паспорт",
                TemplateMetadata = templateMetadatas,

            };
            DataTemplates.Add(testScannerDataTemplates);

            testScannerDataTemplates = new ScannerDataTemplate();
            templateMetadatas = new List<TemplateMetadata>();
            templateMetadatas.Add(new TemplateMetadata
            {
                Name = "Серия",
                Required = true,
            });
            templateMetadatas.Add(new TemplateMetadata
            {
                Name = "Номер",
                Required = true,
            });
            templateMetadatas.Add(new TemplateMetadata
            {
                Name = "ФИО",
                Required = true,
            });
            templateMetadatas.Add(new TemplateMetadata
            {
                Name = "ФИО отца",
                Required = true,
            });
            templateMetadatas.Add(new TemplateMetadata
            {
                Name = "ФИО матери",
                Required = true,
            });
            templateMetadatas.Add(new TemplateMetadata
            {
                Name = "Дата выдачи",
                Required = false,
            });
            templateMetadatas.Add(new TemplateMetadata
            {
                Name = "Дата рождения",
                Required = false,
            });
            templateMetadatas.Add(new TemplateMetadata
            {
                Name = "Кем выдано",
                Required = false,
            });

            testScannerDataTemplates = new ScannerDataTemplate
            {
                DocumentType = "Свидетельство",
                TemplateMetadata = templateMetadatas,

            };
            DataTemplates.Add(testScannerDataTemplates);

            testScannerDataTemplates = new ScannerDataTemplate();
            templateMetadatas = new List<TemplateMetadata>();
            templateMetadatas.Add(new TemplateMetadata
            {
                Name = "Серия",
                Required = true,
            });
            templateMetadatas.Add(new TemplateMetadata
            {
                Name = "Номер",
                Required = true,
            });
            templateMetadatas.Add(new TemplateMetadata
            {
                Name = "ФИО",
                Required = true,
            });
            templateMetadatas.Add(new TemplateMetadata
            {
                Name = "Дата выдачи",
                Required = true,
            });
            templateMetadatas.Add(new TemplateMetadata
            {
                Name = "Наименование УЗ",
                Required = true,
            });
            templateMetadatas.Add(new TemplateMetadata
            {
                Name = "Специальность",
                Required = false,
            });
            templateMetadatas.Add(new TemplateMetadata
            {
                Name = "Специализация",
                Required = false,
            });

            testScannerDataTemplates = new ScannerDataTemplate
            {
                DocumentType = "Диплом",
                TemplateMetadata = templateMetadatas,

            };
            DataTemplates.Add(testScannerDataTemplates);


            #endregion
        }
    }
}
