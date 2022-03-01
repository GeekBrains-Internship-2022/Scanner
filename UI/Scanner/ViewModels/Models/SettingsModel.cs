using Scanner.Models.Base;

namespace Scanner.ViewModels.Models
{
    class SettingsModel : Entity
    {

        public class Rootobject
        {
            public Directories Directories { get; set; }
            public Rabbitmq RabbitMQ { get; set; }
            public Fileservice FileService { get; set; }
            public Serilog Serilog { get; set; }
            public Connectionstrings ConnectionStrings { get; set; }
        }

        public class Directories
        {
            public string ObserverPath { get; set; }
            public string StorageDirectory { get; set; }
        }

        public class Rabbitmq
        {
            public string Uri { get; set; }
            public string Queue { get; set; }
            public string Login { get; set; }
            public string Password { get; set; }
        }

        public class Fileservice
        {
            public string DestinationPath { get; set; }
        }

        public class Serilog
        {
            public string[] Using { get; set; }
            public Levelswitches LevelSwitches { get; set; }
            public Minimumlevel MinimumLevel { get; set; }
            public WritetoSublogger WriteToSublogger { get; set; }
            public WritetoAsync WriteToAsync { get; set; }
            public string[] Enrich { get; set; }
            public Properties Properties { get; set; }
        }

        public class Levelswitches
        {
            public string controlSwitch { get; set; }
        }

        public class Minimumlevel
        {
            public string Default { get; set; }
            public Override Override { get; set; }
        }

        public class Override
        {
            public string Microsoft { get; set; }
            public string MyAppSomethingTricky { get; set; }
        }

        public class WritetoSublogger
        {
            public string Name { get; set; }
            public Args Args { get; set; }
        }

        public class Args
        {
            public Configurelogger configureLogger { get; set; }
            public string restrictedToMinimumLevel { get; set; }
            public string levelSwitch { get; set; }
        }

        public class Configurelogger
        {
            public string MinimumLevel { get; set; }
            public Writeto[] WriteTo { get; set; }
        }

        public class Writeto
        {
            public string Name { get; set; }
            public Args1 Args { get; set; }
        }

        public class Args1
        {
            public string outputTemplate { get; set; }
            public string theme { get; set; }
            public string logDirectory { get; set; }
            public int fileSizeLimitBytes { get; set; }
            public string pathFormat { get; set; }
        }

        public class WritetoAsync
        {
            public string Name { get; set; }
            public Args2 Args { get; set; }
        }

        public class Args2
        {
            public Configure[] configure { get; set; }
        }

        public class Configure
        {
            public string Name { get; set; }
            public Args3 Args { get; set; }
        }

        public class Args3
        {
            public string path { get; set; }
            public string outputTemplate { get; set; }
        }

        public class Properties
        {
            public string Application { get; set; }
        }

        public class Connectionstrings
        {
            public string Default { get; set; }
        }
    }
}
