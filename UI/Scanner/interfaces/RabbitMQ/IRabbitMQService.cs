using Scanner.Models;

namespace Scanner.interfaces.RabbitMQ
{
    public interface IRabbitMQService
    {
        void Publish(Document document);
    }
}