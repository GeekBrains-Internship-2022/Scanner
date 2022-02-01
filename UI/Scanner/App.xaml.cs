using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Scanner.Service;

using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Scanner.interfaces;
using Scanner.interfaces.RabbitMQ;
using Scanner.Service.RabbitMQ;
using Serilog;

namespace Scanner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IHost _Hosting;

        public static IServiceProvider Services => Hosting.Services;

        public static IHost Hosting => _Hosting
            ??= Host.CreateDefaultBuilder(Environment.GetCommandLineArgs())
                .ConfigureAppConfiguration(opt => opt.AddJsonFile("appsettings.json"))
                .ConfigureServices(ConfigureServices)
                .UseSerilog((host, log) => log.ReadFrom.Configuration(host.Configuration))
                .Build();

        private static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
        {
            services.AddSingleton<ObserverService>();                   //  Сервис мониторинга каталога

            //  Сервис файлов
            services.AddSingleton<IFileService, FileService>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<IFileService>>();
                var destPath = host.Configuration["FileService:DestinationPath"];

                return new FileService(logger, destPath);
            });

            #region Сервис кролика

            services.AddSingleton<IRabbitMQService>(sp =>
            {
                var connection = sp.GetRequiredService<IRabbitMQConnection>();
                var logger = sp.GetRequiredService<ILogger<RabbitMQService>>();
                var queueName = host.Configuration["RabbitMQ:Queue"];

                return new RabbitMQService(connection, logger, queueName);
            });

            services.AddSingleton<IRabbitMQConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<RabbitMQConnection>>();
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(host.Configuration["RabbitMQ:Uri"]),
                    UserName = host.Configuration["RabbitMQ:Login"],
                    Password = host.Configuration["RabbitMQ:Password"]
                };

                return new RabbitMQConnection(factory, logger);
            });

            #endregion 

            //services.AddSingleton<MainWindowViewModel>();
            //services.AddSingleton<ITaskbarIcon, TaskBarNotifyIcon>();
            //services.AddSingleton<ProgramData>();
            //services.AddTransient<IMailService, SmtpMailService>();
            //services.AddSingleton<IEncryptorService, Rfc2898Encryptor>();
            //string sql_string = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|Database1.mdf;Integrated Security=True";
            //services.AddDbContext<MailSenderDB>(opt => opt.UseSqlite("Filename=MySupperDataBase.db"));
            //services.AddSingleton<IStore<Recipient>, RecipientsStoreInDB>();
            //services.AddSingleton<IStore<Message>, MessagesStoreInDB>();
            //services.AddSingleton<IStore<Sender>, SenderStoreInDB>();
            //services.AddTransient<MailSenderDBInitializer>();
        }

    }
}
