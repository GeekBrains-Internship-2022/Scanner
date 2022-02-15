using Scanner.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scanner.Models
{
    [Table("TemplateMetadata")]
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
