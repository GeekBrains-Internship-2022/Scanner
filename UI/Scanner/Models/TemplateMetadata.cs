using Scanner.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scanner.Models
{    
    public class TemplateMetadata: Entity
    {
        public ScannerDataTemplate ScannerDataTemplate { get; set; }
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
