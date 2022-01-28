﻿using Scanner.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scanner.Models
{
    public class Document : Entity
    {
        /// <summary>
        /// Тип документа
        /// </summary>
        public string DocumentType { get; set; }
        /// <summary>
        /// Дата индексирования документа
        /// </summary>
        public DateTime IndexingDate { get; set; }
        /// <summary>
        /// Перечисление метаданных документа
        /// </summary>
        public Dictionary<string, IEnumerable<string>> Metadata { get; set; }
    }
}