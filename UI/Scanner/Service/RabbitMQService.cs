using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

using Scanner.interfaces;
using Scanner.Models;

using System;
using System.Text;
using System.Text.Json;

namespace Scanner.Service
{
    public class RabbitMqService : IDataBusService
    {
        public IDataBusClient GetClient(Uri uri, string userName, string password) =>
            new RabbitMqClient(uri, userName, password);
    }

    internal class RabbitMqClient : IDataBusClient
    {
        private readonly ConnectionFactory _Factory;
        private readonly ILogger<IDataBusClient> _Logger = App.Services.GetRequiredService<ILogger<IDataBusClient>>();

        public RabbitMqClient(Uri uri, string userName, string password)
        {
            _Factory = new ConnectionFactory
            {
                Uri = uri,
                UserName = userName,
                Password = password
            };
        }

        public void SendData(Document document, string queue)
        {
            if (document is null)
            {
                return;
                //throw new NullReferenceException();
            }

            _Logger.LogInformation("Connecting to Rabbit");

            try
            {
                using var connection = _Factory.CreateConnection();
                using var channel = connection.CreateModel();
                {
                    _Logger.LogInformation($"Connected. Declare queue: {queue}.");
                    channel.QueueDeclare(queue: queue,
                        arguments: null,
                        durable: true,
                        exclusive: false,
                        autoDelete: false);


                    var doc = JsonSerializer.Serialize(document);
                    var body = Encoding.UTF8.GetBytes(doc);

                    _Logger.LogInformation("Publish");

                    channel.BasicPublish(exchange: "",
                        routingKey: "",
                        basicProperties: null,
                        body: body);

                }
            }
            catch (BrokerUnreachableException e)
            {
                _Logger.LogError(e.Message);
                throw;
            }
            catch (NotSupportedException e)
            {
                _Logger.LogError(e.Message);
                throw;
            }
            catch (EncoderFallbackException e)
            {
                _Logger.LogError(e.Message);
                throw;
            }
        }
    }
}