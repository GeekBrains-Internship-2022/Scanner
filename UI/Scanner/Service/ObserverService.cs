using Microsoft.Extensions.Logging;

using System;
using System.IO;
using System.Threading;

namespace Scanner.Service
{
    public class ObserverService
    {
        private readonly ILogger _Logger;
        private readonly CancellationToken _Token;

        public delegate void ObserverHandler(string message);
        public event ObserverHandler Notify;

        public ObserverService(ILogger<ObserverService> logger, CancellationToken token = default)
        {
            _Logger = logger;
            _Token = token;
        }

        public void Start(string path)
        {
            if (!Directory.Exists(path))                 //  TODO: Пока нет ui, потом удалить
                Directory.CreateDirectory(path);

            using var watcher = new FileSystemWatcher()
            {
                Path = path,
                NotifyFilter = NotifyFilters.FileName,
                Filter = "*.pdf",
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };

            _Logger.LogInformation($"Observer is created\nObserved path: {watcher.Path}");

            watcher.Created += OnCreated;
            watcher.Renamed += OnRenamed;
            watcher.Changed += OnChanged;
            watcher.Deleted += OnDeleted;
            watcher.Disposed += OnDisposed;
            watcher.Error += OnError;

            while (!_Token.IsCancellationRequested) { }     //  Костыль, чтобы сервис не выгружался
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(e.FullPath))
            {
                //throw new FileNotFoundException(e.FullPath);
                return;
            }

            var path = Path.GetFullPath(e.FullPath);

            _Logger.LogInformation($"Created: \"{path}\"");
            Notify?.Invoke(path);
        }

        private void OnRenamed(object sender, RenamedEventArgs e) => _Logger.LogInformation($"Renamed: \"{e.OldName}\" -> \"{e.Name}\"");
        private void OnChanged(object sender, FileSystemEventArgs e) => _Logger.LogInformation($"Changed: \"{e.FullPath}\"");
        private void OnDeleted(object sender, FileSystemEventArgs e) => _Logger.LogInformation($"Deleted: \"{e.FullPath}\"");
        private void OnError(object sender, ErrorEventArgs e) => _Logger.LogError(e.GetException().Message);
        private void OnDisposed(object sender, EventArgs e) => _Logger.LogInformation("Observer Service is Disposed");
    }
}