using System.Threading.Tasks;
using Scanner.Models;

namespace Scanner.interfaces.RabbitMQ
{
    public interface IRabbitMQService
    {
        void Publish(FileData fileData, int templateId);
        Task PublishAsync(FileData fileData, int templateId);
    }
}