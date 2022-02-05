using Microsoft.Extensions.Logging;

using RabbitMQ.Client;

using Scanner.interfaces.RabbitMQ;
using Scanner.Models;

using System.Text;
using System.Text.Json;

namespace Scanner.Service.RabbitMQ
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly IRabbitMQConnection _Connection;
        private readonly ILogger<RabbitMQService> _Logger;
        private readonly string _QueueName;

        public RabbitMQService(IRabbitMQConnection connection, ILogger<RabbitMQService> logger, string queueName)
        {
            _Connection = connection;
            _Logger = logger;
            _QueueName = queueName;
        }

        public void Publish(Document document)
        {
            if (!_Connection.IsConnected)
                _Connection.TryConnect();

            using var channel = _Connection.CreateModel();
            _Logger.LogInformation(
                $"Declaring RabbitMQ exchange to publish:\nId:\t{document.Id}\nDocument Type:\t{document.DocumentType}");

            channel.QueueDeclare(queue: _QueueName,
                arguments: null,
                durable: true,
                exclusive: false,
                autoDelete: false);


            var doc = JsonSerializer.Serialize(document);
            var body = Encoding.UTF8.GetBytes(doc);

            _Logger.LogInformation($"Publish to {_QueueName} queue");

            channel.BasicPublish(exchange: "",
                routingKey: "",
                basicProperties: null,
                body: body);
        }
    }
}
