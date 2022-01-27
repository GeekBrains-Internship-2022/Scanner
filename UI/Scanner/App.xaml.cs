using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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
               .ConfigureServices(ConfigureServices)
               .Build();

        private static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
        {
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
