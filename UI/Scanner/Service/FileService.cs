using Microsoft.Extensions.Logging;

using Scanner.interfaces;
using Scanner.Models;

using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Scanner.Service
{
    public class FileService : IFileService
    {
        private readonly ILogger<IFileService> _Logger;
        private readonly IConfiguration _Configuration;
        private readonly string _FileExtension = ".pdf";

        public FileService(ILogger<IFileService> logger, IConfiguration configuration)
        {
            _Logger = logger;
            _Configuration = configuration;
        }

        public void Move(string sourceFileName, string fileName)
        {
            if (!File.Exists(sourceFileName))
            {
                _Logger.LogInformation($"File: {sourceFileName} not found");
                throw new FileNotFoundException();
            }

            var storage = _Configuration["Directories:StorageDirectory"];
            var newFileName = Path.Combine(storage, fileName + _FileExtension);
            //var path = Path.Combine(_DestPath, fileName + _FileExtension);

            try
            {
                if (!Directory.Exists(storage))
                    Directory.CreateDirectory(storage);   //  TODO: пока нет UI, потом удалить

                File.Move(sourceFileName, newFileName);
            }
            catch (IOException e)
            {
                _Logger.LogError(e.Message);
                throw new IOException(e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                _Logger.LogError(e.Message);
                throw new UnauthorizedAccessException(e.Message);
            }
        }

        public FileData CreateFileData(string path, string documentType) => new FileData
            {FilePath = path, Document = new Document {DocumentType = documentType}};
    }
}
