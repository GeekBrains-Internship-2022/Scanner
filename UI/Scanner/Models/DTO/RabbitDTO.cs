using System.Collections.Generic;

namespace Scanner.Models.DTO
{
    public class RabbitDTO
    {
        public int MessageId { get; set; } = 0;
        public string DocumentType { get; set; }
        public int OutputTemplateId { get; set; }
        public Dictionary<string, IEnumerable<string>> Data { get; set; }
    }
}
