using System;
using System.Collections.Generic;
using System.Text;

namespace Scanner.Models
{
    public class Document
    {
        /// <summary>
        /// Тип документа
        /// </summary>
        public string DocumentType;
        /// <summary>
        /// Дата индексирования документа
        /// </summary>
        public DateTime IndexingDate;
        /// <summary>
        /// Перечисление метаданных документа
        /// </summary>
        public Dictionary<string, IEnumerable<string>> Metadata;
    }
}
