using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Scanner.Service
{
    public class ObserverService
    {
        private readonly string _Path;
        private readonly ILogger<ObserverService> _Logger;

        public delegate void ObserverHandler(string message);
        public event ObserverHandler Notify;

        public ObserverService(string path, ILogger<ObserverService> logger)
        {
            _Path = path;
            _Logger = logger;
        }

        public void Start()
        {
            using var watcher = new FileSystemWatcher()
            {
                Path = _Path,
                NotifyFilter = NotifyFilters.FileName,
                Filter = "*.pdf",
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };
            
            watcher.Created += OnCreated;
            watcher.Renamed += OnRenamed;
            watcher.Changed += OnChanged;
            watcher.Deleted += OnDeleted;
            watcher.Error += OnError;
            watcher.Disposed += OnDisposed;
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            _Logger.LogInformation($"Created: {e.FullPath}");
            Notify?.Invoke(e.FullPath);
        }

        private void OnRenamed(object sender, RenamedEventArgs e) => _Logger.LogInformation($"Renamed: {e.OldName} -> {e.Name}");
        private void OnChanged(object sender, FileSystemEventArgs e) => _Logger.LogInformation($"Changed: {e.FullPath}");
        private void OnDeleted(object sender, FileSystemEventArgs e) => _Logger.LogInformation($"Deleted: {e.FullPath}");
        private void OnError(object sender, ErrorEventArgs e) => _Logger.LogError(e.GetException().Message);
        private void OnDisposed(object sender, EventArgs e) => _Logger.LogInformation("Observer Service is Disposed");
    }
}