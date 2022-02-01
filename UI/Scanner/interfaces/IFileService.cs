using Scanner.Models;

using System;

namespace Scanner.interfaces
{
    public interface IFileDataBuilder
    {
        IFileDataBuilder Path(string path);
        IFileDataBuilder Name(string name);
        IFileDataBuilder Description(string description);
        IFileDataBuilder Time(DateTime time);
        IFileDataBuilder IsIndexed(bool isIndexed);
        IFileDataBuilder Document(Document document);
        FileData Build();
    }

    public interface IFileService
    {
        void Move(string filePath, string fileName);
        FileData CreateFileData(string path, string documentType);
    }
}
