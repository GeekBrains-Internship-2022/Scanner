using Scanner.Models;

using System;

namespace Scanner.interfaces
{
    public interface IFileService
    {
        void Move(string filePath, string fileName);
        FileData CreateFileData(string path, string documentType);
    }
}
