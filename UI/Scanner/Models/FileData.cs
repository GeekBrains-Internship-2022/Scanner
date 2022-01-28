using Scanner.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Scanner.Models
{

    public class FileData: Entity
    {
        /// <summary>
        /// Уникальный идентификатор файла в БД
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Guid { get; set; }
        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// Название документа
        /// </summary>
        public string DocumentName { get; set; }
        /// <summary>
        /// Описание документа
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Дата добавления файла в БД
        /// </summary>
        public DateTime DateAdded { get; set; }
        /// <summary>
        /// Состояния индексирования
        /// </summary>
        public bool Indexed { get; set; }
        /// <summary>
        /// Шаблон данных
        /// </summary>
        public Document Document { get; set; }
    }
}
