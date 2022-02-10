using Scanner.interfaces;
using Scanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner.Infrastructure
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
}
