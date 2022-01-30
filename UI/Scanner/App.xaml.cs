using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Scanner.Service;

using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
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
            services.AddSingleton<ObserverService>();       //  Сервис мониторинга каталога
            services.AddSingleton<RabbitMqService>();       //  Сервис кролика

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
