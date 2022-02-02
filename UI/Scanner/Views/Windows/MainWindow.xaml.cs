using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Scanner.interfaces;
using Scanner.interfaces.RabbitMQ;
using Scanner.Models;
using Scanner.Service;

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Scanner.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MethodForTest();
        }

        #region For test without ui

        private static readonly IConfiguration __Configuration = App.Services.GetRequiredService<IConfiguration>();

        private static void MethodForTest()
        {
            //ObserverTest();
            //FileServiceTest();
            //RabbitTest();
        }

        private static void RabbitTest()
        {
            var rabbit = App.Services.GetRequiredService<IRabbitMQService>();
            var doc = new Document
            {
                DocumentType = "Test Document",
                IndexingDate = DateTime.Now,
                Metadata = Enumerable
                    .Range(1, 5)
                    .Select(i => new DocumentMetadata
                    {
                        Id = i,
                        Data = $"Data-{i}",
                        Name = $"Name-{i}"
                    }).ToArray()
            };

            rabbit.Publish(doc);
        }

        private static void ObserverTest()
        {
            var observer = App.Services.GetRequiredService<ObserverService>();
            var path = __Configuration["ObserverPath"];
            var task = new Task(() => observer.Start(path));

            observer.Notify += OnNotify;
            task.Start();

            void OnNotify(string message) => Debug.WriteLine(message);
        }

        private static void FileServiceTest()
        {
            var filePath = @"D:\111\111.pdf";
            var fileService = App.Services.GetRequiredService<IFileService>();
            var fileData = fileService.CreateFileData(filePath, "Pasport");

            fileService.Move(filePath, fileName: fileData.Guid.ToString());
        }

        #endregion
    }
}
