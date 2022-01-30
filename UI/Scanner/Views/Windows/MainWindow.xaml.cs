using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Scanner.Service;

using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace Scanner
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

        private static void MethodForTest()
        {
            var logger = App.Services.GetRequiredService<ILogger<ObserverService>>();
            var configuration = App.Services.GetRequiredService<IConfiguration>();

            var path = configuration["ObserverPath"];

            var observer = new ObserverService(logger);
            var task = new Task(() =>
            {
                observer.Start(path);
            });
            observer.Notify += OnNotify;
            task.Start();

            void OnNotify(string message)
            {
                Debug.WriteLine(message);
            }
        }
    }
}
