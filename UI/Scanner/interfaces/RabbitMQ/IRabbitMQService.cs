using Scanner.Models;

namespace Scanner.interfaces.RabbitMQ
{
    public interface IRabbitMQService
    {
        void Publish(FileData fileData, int templateId);
    }
}