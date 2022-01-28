using System;
using System.Collections.Generic;
using System.Text;

namespace Scanner.Models
{

    public class FileData
    {
        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string FilePath;
        /// <summary>
        /// Название документа
        /// </summary>
        public string DocumentName;
        /// <summary>
        /// Описание документа
        /// </summary>
        public string Description;
        /// <summary>
        /// Уникальный идентификатор файла в БД
        /// </summary>
        public string GUID;
        /// <summary>
        /// Дата добавления файла в БД
        /// </summary>
        public DateTime DateAdded;
        /// <summary>
        /// Состояния индексирования
        /// </summary>
        public bool Indexed;
        /// <summary>
        /// Шаблон данных
        /// </summary>
        public Document Document;
    }
}
