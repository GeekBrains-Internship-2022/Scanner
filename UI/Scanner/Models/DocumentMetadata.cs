using Scanner.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scanner.Models
{
    [Table("Metadata")]
    public class DocumentMetadata: Entity
    {
        public string Name { get; set; }

        public string Data { get; set; }
    }
}
