using System;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Scanner.Data;
using Scanner.Data.Stores.InDB;
using Scanner.interfaces;
using Scanner.interfaces.RabbitMQ;
using Scanner.Models;
using Scanner.Service;
using Scanner.Service.RabbitMQ;
using Scanner.ViewModels;

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
                .ConfigureHostConfiguration(cfg => cfg
                   .AddJsonFile("appconfig.json", true, true))
                .ConfigureAppConfiguration(cfg => cfg.AddJsonFile("appconfig.json", true, true))
                .ConfigureServices(ConfigureServices)
                .UseSerilog((host, log) => log.ReadFrom.Configuration(host.Configuration))
                .Build();

        private static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
        {
            services.AddSingleton<IObserverService, ObserverService>();                 //  Сервис мониторинга каталога
            services.AddSingleton<IRabbitMQService, RabbitMQService>();                 //  Сервис кролика
            services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();           //  Сервис постоянного подключения кролика

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<SettingsWindowViewModel>();
            services.AddSingleton<ViewModelTestDB>();

            var path = host.Configuration.GetConnectionString("Default");
            services.AddDbContext<ScannerDB>(opt => opt.UseSqlite(path));
            services.AddScoped(typeof(IStore<>), typeof(DbStore<>));                    //  Хранилище
            services.AddTransient<ScannerDbInitializer>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Services.GetRequiredService<ScannerDbInitializer>().Initialize();
            base.OnStartup(e);
        }

    }
}
