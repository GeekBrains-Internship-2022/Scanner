using System.Threading;
using System.Threading.Tasks;
using Scanner.Service;

namespace Scanner.interfaces
{
    public interface IObserverService
    {
        event ObserverService.ObserverHandler NotifyOnCreated;
        event ObserverService.RenamedEventHandler NotifyOnRenamed;
        event ObserverService.ObserverHandler NotifyOnChanged;
        event ObserverService.ObserverHandler NotifyOnDeleted;
        event ObserverService.ObserverHandler NotifyOnError;

        Task StartAsync(CancellationToken cancel = default);
    }
}