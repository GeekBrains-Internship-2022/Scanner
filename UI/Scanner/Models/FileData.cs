using Scanner.Models.Base;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scanner.Models
{

    public class FileData : Entity
    {
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
        /// Файл был проверен администратором
        /// </summary>
        public bool Checked {get;set;}
        /// <summary>
        /// Шаблон данных
        /// </summary>
        public Document Document { get; set; }
    }
}
