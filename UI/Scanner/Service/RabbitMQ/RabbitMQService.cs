using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using RabbitMQ.Client;

using Scanner.interfaces.RabbitMQ;
using Scanner.Models;
using Scanner.Models.DTO;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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

        public void Publish(FileData fileData, int templateId)
        {
            if (!_Connection.IsConnected)
                _Connection.TryConnect();

            var queueName = _Configuration["RabbitMQ:Queue"];
            var exchangeName = _Configuration["RabbitMQ:Exchange"];
            var routingKey = _Configuration["RabbitMQ:RoutingKey"];

            using var channel = _Connection.CreateModel();
            _Logger.LogInformation(
                $"Declaring RabbitMQ exchange to publish:\nId:\t{fileData.Id}\nDocument Type:\t{fileData.Document.DocumentType}");

            channel.ExchangeDeclare(exchange: exchangeName,
                type: "direct",
                durable: true);

            channel.QueueDeclare(queue: queueName,
                arguments: null,
                durable: true,
                exclusive: false,
                autoDelete: false);

            channel.QueueBind(queue: queueName,
                exchange: exchangeName,
                routingKey: routingKey);

            var docType = fileData.Document.DocumentType;
            var data = fileData.Document.Metadata.ToLookup(n => n.Name, d => d.Data)
                .ToDictionary(k => k.Key, d => (IEnumerable<string>)d);

            var dto = new RabbitDTO
            {
                DocumentType = docType,
                OutputTemplateId = templateId,
                Data = data
            };

            var doc = JsonSerializer.Serialize(dto);
            var body = Encoding.UTF8.GetBytes(doc);

            _Logger.LogInformation($"Publish to {queueName} queue");

            channel.BasicPublish(exchange: exchangeName,
                    routingKey: routingKey,
                    basicProperties: null,
                    body: body);
        }

        public async Task PublishAsync(FileData fileData, int templateId) => await Task.Run(() => Publish(fileData, templateId));
    }
}