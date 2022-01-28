using System.IO;

namespace Scanner.Service
{
    public class ObserverService
    {
        private readonly string _Path;

        public ObserverService(string path) => _Path = path;

        void Start()
        {
            using var watcher = new FileSystemWatcher(_Path)
            {
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "*.pdf",
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
            };

            watcher.Created += OnCreated;
            watcher.Error += OnError;
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
