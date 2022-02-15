using Scanner.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scanner.Models
{    
    public class DocumentMetadata: Entity
    {
        public Document Document { get; set; }
        public string Name { get; set; }

        public string Data { get; set; }
    }
}
