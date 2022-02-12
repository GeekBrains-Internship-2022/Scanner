using Microsoft.Extensions.Logging;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Scanner.interfaces;

namespace Scanner.Service
{
    public class ObserverService : IObserverService
    {
        private readonly ILogger _Logger;
        private readonly IConfiguration _Configuration;

        public delegate void ObserverHandler(string path);
        public delegate void RenamedEventHandler(string oldPath, string path);
        public event RenamedEventHandler NotifyOnRenamed;
        public event ObserverHandler NotifyOnCreated;
        public event ObserverHandler NotifyOnChanged;
        public event ObserverHandler NotifyOnDeleted;
        public event ObserverHandler NotifyOnError;


        public ObserverService(ILogger<ObserverService> logger, IConfiguration configuration)
        {
            _Logger = logger;
            _Configuration = configuration;
        }

        private void Start()
        {
            if (string.IsNullOrEmpty(_Configuration["Directories:ObserverPath"]))
            {
                _Logger.LogError("Key not found for ObserverPath");
                throw new NullReferenceException("Key not found for ObserverPath");
            }
            var path = _Configuration["Directories:ObserverPath"];

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

            while (true) { }     //  Костыль, чтобы сервис не выгружался
        }

        public async Task StartAsync(CancellationToken cancel = default) => await Task.Run(Start, cancel);

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(e.FullPath))
                return;

            var path = Path.GetFullPath(e.FullPath);

            _Logger.LogInformation($"Created: \"{path}\"");
            NotifyOnCreated?.Invoke(path);
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            if (!File.Exists(e.FullPath))
                return;

            var oldPath = Path.GetFullPath(e.OldFullPath);
            var currentPath = Path.GetFullPath(e.FullPath);

            _Logger.LogInformation($"Renamed: \"{e.OldName}\" -> \"{e.Name}\"");
            
            NotifyOnRenamed?.Invoke(oldPath, currentPath);
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(e.FullPath))
                return;

            var path = Path.GetFullPath(e.FullPath);

            _Logger.LogInformation($"Changed: \"{e.FullPath}\"");
            
            NotifyOnChanged?.Invoke(path);
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            var path = Path.GetFullPath(e.FullPath);

            _Logger.LogInformation($"Deleted: \"{e.FullPath}\"");

            NotifyOnDeleted?.Invoke(path);
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            _Logger.LogError(e.GetException().Message);

            NotifyOnError?.Invoke(e.GetException().Message);
        }

        private void OnDisposed(object sender, EventArgs e) => Debug.WriteLine("Observer Service is Disposed");
    }
}