using System;
using Scanner.Models;
using Scanner.Service;

namespace Scanner.interfaces
{
    public interface IDataBusService
    {
        IDataBusClient GetClient(Uri uri, string userName, string password) => new RabbitMqClient(uri, userName, password);
    }

    public interface IDataBusClient
    {
        void SendData(Document document);
    }
}