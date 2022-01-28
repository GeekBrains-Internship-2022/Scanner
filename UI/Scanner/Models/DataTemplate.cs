using Microsoft.EntityFrameworkCore;
using Scanner.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scanner.Models
{
    [Index(nameof(DocumentType), IsUnique = true)]
    public class DataTemplate: Entity
    {
        /// <summary>
        /// Уникальное имя шаблона
        /// </summary>
        public string DocumentType;
        /// <summary>
        /// Шаблон метаданных
        /// </summary>
        public Dictionary<string, bool> TemplateMetadata;
    }
}
