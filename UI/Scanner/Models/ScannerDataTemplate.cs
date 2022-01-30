using Microsoft.EntityFrameworkCore;
using Scanner.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Scanner.Models
{
    [Index(nameof(DocumentType), IsUnique = true)]
    public class ScannerDataTemplate: Entity
    {
        /// <summary>
        /// Уникальное имя шаблона
        /// </summary>
        [Required,MaxLength(25)]
        public string DocumentType { get; set; }
        /// <summary>
        /// Шаблон метаданных
        /// </summary>
        public ICollection<TemplateMetadata> TemplateMetadata { get; set; }
    }
}
