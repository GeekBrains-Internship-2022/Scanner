using Microsoft.Extensions.Logging;

using Scanner.interfaces;
using Scanner.Models;

using System;
using System.IO;

namespace Scanner.Service
{
    public class FileDataBuilder : IFileDataBuilder
    {
        private readonly FileData _Data = new();

        public IFileDataBuilder Path(string path)
        {
            _Data.FilePath = path;
            return this;
        }

        public IFileDataBuilder Name(string name)
        {
            _Data.DocumentName = name;
            return this;
        }

        public IFileDataBuilder Description(string description)
        {
            _Data.Description = description;
            return this;
        }

        public IFileDataBuilder Time(DateTime time)
        {
            _Data.DateAdded = time;
            return this;
        }

        public IFileDataBuilder IsIndexed(bool isIndexed)
        {
            _Data.Indexed = isIndexed;
            return this;
        }

        public IFileDataBuilder Document(Document document)
        {
            _Data.Document = document;
            return this;
        }

        public FileData Build() => _Data;
    }

    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _Logger;

        public FileService(ILogger<FileService> logger) => _Logger = logger;
        
        public void Move(string sourceFileName, string destFileName)
        {
            if (!File.Exists(sourceFileName))
            {
                _Logger.LogInformation($"File: {sourceFileName} not found");
                throw new FileNotFoundException();
            }

            try
            {
                File.Move(sourceFileName, destFileName);
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
                .Document(new Document {DocumentType = documentType})
                .Build();
        }
    }
}
