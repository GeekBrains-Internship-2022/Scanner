using Microsoft.Extensions.Logging;
using Scanner.Infrastructure;
using Scanner.interfaces;
using Scanner.Models;

using System;
using System.IO;

namespace Scanner.Service
{
    

    public class FileService : IFileService
    {
        private readonly ILogger<IFileService> _Logger;
        private readonly string _DestPath;
        private readonly string _FileExtension = ".pdf";

        public FileService(ILogger<IFileService> logger, string destPath)
        {
            _Logger = logger;
            if (string.IsNullOrEmpty(destPath))
                destPath = ".\\scanfiles";
            _DestPath = Path.GetFullPath(destPath);
        }

        public void Move(string sourceFileName, string fileName)
        {
            if (!File.Exists(sourceFileName))
            {
                _Logger.LogInformation($"File: {sourceFileName} not found");
                throw new FileNotFoundException();
            }

            var path = Path.Combine(_DestPath, fileName + _FileExtension);

            try
            {
                if (!Directory.Exists(_DestPath))
                    Directory.CreateDirectory(_DestPath);   //  TODO: пока нет UI, потом удалить

                File.Move(sourceFileName, path);
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

        public FileData CreateFileData(string path, string documentType)
        {
            var data = new FileDataBuilder();

            return data.Path(path)
                .Document(new Document { DocumentType = documentType })
                .Build();
        }
    }
}
