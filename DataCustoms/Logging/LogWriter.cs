namespace DataCustoms.Logging
{
    public class LogWriter : IDisposable
    {
        public string DebugFile;
        public string LogFile;
        private static LogWriter _instance = new("logOutput");
        public static LogWriter Main
        {
            get => _instance;
            set => _instance = value;
        }
        private static readonly Mercury _logger = new();
        internal static Mercury Logger => _logger;
        private bool _disposedValue;
        private readonly FileStream _logOutput;
        private readonly FileStream _debugOutput;
        private readonly StreamWriter _logWriter;
        private readonly StreamWriter _debugWriter;
        public LogWriter(string outputFileName)
        {
            string path = Directory.GetCurrentDirectory();

            string outputDir = Path.Combine(path, "output");
            Directory.CreateDirectory(outputDir);

            LogFile = Path.Combine(outputDir, $"{outputFileName}.log");
            _logOutput = File.Create(LogFile);
            _logWriter = new StreamWriter(_logOutput) { AutoFlush = true };
            _logWriter.WriteLine("[Initialized LogWriter]");

            DebugFile = Path.Combine(outputDir, "debug.log");
            _debugOutput = File.Create(DebugFile);
            _debugWriter = new StreamWriter(_debugOutput) { AutoFlush = true };
            _debugWriter.WriteLine("[Initialized DebugWriter]");
        }
        public void RegisterLogSource(ILogger logSource) => logSource.LogEvent += OnLog;

        public bool OnLog(ILogger source, AbstractLogArgs args)
        {
            bool written = false;
            try
            {
                _debugWriter.WriteLine($"[From : {source.Source,10}]");
                _debugWriter.WriteLine($"[At   : {DateTime.Now,10}]");
                _debugWriter.WriteLine(args.ToString());
                _debugWriter.WriteLine();
                if (args.Level != LogLevel.Debug)
                {
                    _logWriter.WriteLine(args.ToString());
                    _logWriter.WriteLine();
                    if (!source.IsDiscrete)
                    {
                        if (args.Level == LogLevel.Error) Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(args.ToString());
                        Console.WriteLine();
                        Console.ResetColor();
                    }
                }

                written = true;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ResetColor();
            }
            return written;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                _logOutput.Dispose();
                _logWriter.Dispose();
                _debugOutput.Dispose();
                _debugWriter.Dispose();
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
