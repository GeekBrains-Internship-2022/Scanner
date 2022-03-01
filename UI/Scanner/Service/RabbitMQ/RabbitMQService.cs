using Microsoft.Extensions.Logging;

using RabbitMQ.Client;

using Scanner.interfaces.RabbitMQ;
using Scanner.Models;

using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Scanner.Service.Mapping.DTO;

namespace Scanner.Service.RabbitMQ
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly IRabbitMQConnection _Connection;
        private readonly ILogger<RabbitMQService> _Logger;
        private readonly IConfiguration _Configuration;

        public RabbitMQService(IRabbitMQConnection connection, ILogger<RabbitMQService> logger, IConfiguration configuration)
        {
            _Connection = connection;
            _Logger = logger;
            _Configuration = configuration;
        }

        public void Publish(FileData fileData)
        {
            if (!_Connection.IsConnected)
                _Connection.TryConnect();

            var queueName = _Configuration["RabbitMQ:Queue"];

            using var channel = _Connection.CreateModel();
            _Logger.LogInformation(
                $"Declaring RabbitMQ exchange to publish:\nId:\t{fileData.Id}\nDocument Type:\t{fileData.Document.DocumentType}");

            channel.QueueDeclare(queue: queueName,
                arguments: null,
                durable: true,
                exclusive: false,
                autoDelete: false);

            var doc = JsonSerializer.Serialize(fileData.ToDTO());
            var body = Encoding.UTF8.GetBytes(doc);

            _Logger.LogInformation($"Publish to {queueName} queue");

            channel.BasicPublish(exchange: "",
                routingKey: "",
                basicProperties: null,
                body: body);
        }
    }
}