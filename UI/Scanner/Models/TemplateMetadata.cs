using Scanner.Models.Base;

namespace Scanner.Models
{
    public class TemplateMetadata: Entity
    {
        /// <summary>
        /// Имя метаданных
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Обязательные ли метаданные
        /// </summary>
        public bool Required { get; set; }
    }
}
