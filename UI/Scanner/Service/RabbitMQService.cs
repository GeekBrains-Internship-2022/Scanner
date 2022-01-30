using RabbitMQ.Client;

using Scanner.Models;

using System;
using System.Text;
using System.Text.Json;
using Scanner.interfaces;

namespace Scanner.Service
{
    public class RabbitMqService : IRabbitService
    {
        public IRabbitClient GetClient(Uri uri, string userName, string password) => new RabbitClient(uri, userName, password);
    }

    internal class RabbitClient : IRabbitClient
    {
        private readonly ConnectionFactory _Factory;

        public RabbitClient(Uri uri, string userName, string password)
        {
            _Factory = new ConnectionFactory
            {
                Uri = uri,
                UserName = userName,
                Password = password
            };
        }

        public void SendData(Document document)
        {
            if (document is null)
            {
                return;
                //throw new NullReferenceException();
            }
            using var connection = _Factory.CreateConnection();
            using var channel = connection.CreateModel();
            {
                channel.QueueDeclare(queue: "Robot_IN",
                    arguments: null,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);
                
                var doc = JsonSerializer.Serialize(document);
                var body = Encoding.UTF8.GetBytes(doc);

                channel.BasicPublish(exchange: "",
                    routingKey: "",
                    basicProperties: null,
                    body: body);
            }
        }
    }
}