using System;

namespace Scanner.Models.DTO
{
    public class FileDataDTO
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string DocumentName { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool Indexed { get; set; }
        public bool Checked { get; set; }
        public int DocumentId { get; set; }
    }
}
