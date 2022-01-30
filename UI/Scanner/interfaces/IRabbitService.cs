using System;
using Scanner.Models;
using Scanner.Service;

namespace Scanner.interfaces
{
    public interface IRabbitService
    {
        IRabbitClient GetClient(Uri uri, string userName, string password) => new RabbitClient(uri, userName, password);
    }

    public interface IRabbitClient
    {
        void SendData(Document document);
    }
}