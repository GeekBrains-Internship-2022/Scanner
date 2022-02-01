using Microsoft.Extensions.Logging;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using Scanner.interfaces.RabbitMQ;

using System;
using System.IO;

namespace Scanner.Service.RabbitMQ
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly IConnectionFactory _ConnectionFactory;
        private readonly ILogger<RabbitMQConnection> _Logger;
        private IConnection _Connection;
        private bool _Disposed;

        private readonly object _Lock = new();

        public RabbitMQConnection(IConnectionFactory connectionFactory, ILogger<RabbitMQConnection> logger)
        {
            _ConnectionFactory = connectionFactory;
            _Logger = logger;
        }

        public bool IsConnected => _Connection is not null && _Connection.IsOpen && !_Disposed;

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                var e = "No RabbitMQ connections are available to perform this action";
                _Logger.LogError(e);
                throw new InvalidOperationException(e);
            }

            return _Connection.CreateModel();
        }

        public void Dispose()
        {
            if(_Disposed) return;
            _Disposed = true;
            try
            {
                _Connection.Dispose();
            }
            catch (IOException e)
            {
                _Logger.LogError(e.Message);
            }
        }

        public bool TryConnect()
        {
            _Logger.LogInformation("RabbitMQ Client is trying to connect");

            lock (_Lock)
            {
                _Connection = _ConnectionFactory.CreateConnection();
            }

            if (IsConnected)
            {
                _Connection.ConnectionShutdown += OnConnectionShutdown;
                _Connection.CallbackException += OnCallbackException;
                _Connection.ConnectionBlocked += OnConnectionBlocked;
            }

            return true;
        }

        private void OnConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
        {
            if (_Disposed) return;

            _Logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

            TryConnect();
        }

        private void OnCallbackException(object? sender, CallbackExceptionEventArgs e)
        {
            if (_Disposed) return;
            
            _Logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

            TryConnect();
        }

        private void OnConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            if (_Disposed) return;

            _Logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

            TryConnect();
        }
    }
}