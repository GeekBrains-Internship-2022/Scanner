using Scanner.Models;
using Scanner.Models.DTO;

using System.Collections.Generic;

namespace Scanner.Service.Mapping.DTO
{
    public static class FileDataDTOMapping
    {
        public static FileDataDTO ToDTO(this FileData fileData) => fileData is null
            ? null
            : new FileDataDTO
            {
                Id = fileData.Id,
                Checked = fileData.Checked,
                DateAdded = fileData.DateAdded,
                Description = fileData.Description,
                DocumentId = fileData.Document.Id,
                DocumentName = fileData.DocumentName,
                FilePath = fileData.FilePath,
                Indexed = fileData.Indexed
            };

        public static FileData FromDTO(this FileDataDTO fileData) => fileData is null
            ? null
            : new FileData
            {
                Id = fileData.Id,
                Checked = fileData.Checked,
                DateAdded = fileData.DateAdded,
                Description = fileData.Description,
                Document = new Document {Id = fileData.DocumentId},
                DocumentName = fileData.DocumentName,
                FilePath = fileData.FilePath,
                Indexed = fileData.Indexed
            };

        public static IEnumerable<FileDataDTO> ToDTO(this IEnumerable<FileData> fileData) => fileData.ToDTO();

        public static IEnumerable<FileData> FromDTO(this IEnumerable<FileDataDTO> fileData) => fileData.FromDTO();
    }
}
