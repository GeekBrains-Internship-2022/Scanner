using Scanner.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scanner.Models
{
    public class Document : Entity
    {
        /// <summary>
        /// Тип документа
        /// </summary>
        [MaxLength(25)]
        public string DocumentType { get; set; }
        /// <summary>
        /// Дата индексирования документа
        /// </summary>
        public DateTime IndexingDate { get; set; }// = DateTime.Now;
        /// <summary>
        /// Перечисление метаданных документа
        /// </summary>
        public ICollection<DocumentMetadata> Metadata { get; set; }
    }
}
