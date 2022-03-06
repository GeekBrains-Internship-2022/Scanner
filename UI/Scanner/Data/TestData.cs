using Scanner.Models;

using System.Collections.Generic;

namespace Scanner.Data
{
    internal static class TestData
    {
        public static IEnumerable<ScannerDataTemplate> DataTemplates { get; } = new ScannerDataTemplate[]
        {
            new()
            {
                DocumentType = "Паспорт",
                TemplateMetadata = new List<TemplateMetadata>
                {
                    new() {Name = "Серия", Required = true},
                    new() {Name = "Номер", Required = true},
                    new() {Name = "ФИО", Required = true},
                    new() {Name = "Дата выдачи", Required = false},
                    new() {Name = "Кем выдан", Required = false}
                },
            },
            new()
            {
                DocumentType = "Свидетельство",
                TemplateMetadata = new List<TemplateMetadata>
                {
                    new() {Name = "Серия", Required = true},
                    new() {Name = "Номер", Required = true},
                    new() {Name = "ФИО", Required = true},
                    new() {Name = "ФИО отца", Required = true},
                    new() {Name = "ФИО матери", Required = true},
                    new() {Name = "Дата выдачи", Required = false},
                    new() {Name = "Дата рождения", Required = false},
                    new() {Name = "Кем выдано", Required = false}
                }
            },
            new()
            {
                DocumentType = "Диплом",
                TemplateMetadata = new List<TemplateMetadata>
                {
                    new() {Name = "Серия", Required = true},
                    new() {Name = "Номер", Required = true},
                    new() {Name = "ФИО", Required = true},
                    new() {Name = "Дата выдачи", Required = true},
                    new() {Name = "Наименование УЗ", Required = true},
                    new() {Name = "Специальность", Required = false},
                    new() {Name = "Специализация", Required = false}
                }
            }
        };
    }
}