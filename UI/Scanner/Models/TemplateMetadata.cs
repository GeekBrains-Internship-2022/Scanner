using Scanner.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
