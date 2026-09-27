namespace DataCustoms.Logging
{
    public class LogWriter : IDisposable
    {
        private static LogWriter _instance = new("logOutput");
        public static LogWriter Main
        {
            get => _instance;
            set => _instance = value;
        }
        private bool disposedValue;
        private readonly FileStream logOutput;
        private readonly FileStream debugOutput;
        private readonly StreamWriter logWriter;
        private readonly StreamWriter debugWriter;
        public LogWriter(string outputFileName)
        {
            string path = Directory.GetCurrentDirectory();

            string outputDir = Path.Combine(path, "output");
            Directory.CreateDirectory(outputDir);

            string filepath = Path.Combine(outputDir, $"{outputFileName}.log");
            logOutput = File.Create(filepath);
            logWriter = new StreamWriter(logOutput) { AutoFlush = true };
            logWriter.WriteLine("[Initialized LogWriter]");

            string debugpath = Path.Combine(outputDir, "debug.log");
            debugOutput = File.Create(debugpath);
            debugWriter = new StreamWriter(debugOutput) { AutoFlush = true };
            debugWriter.WriteLine("[Initialized DebugWriter]");
        }
        public void RegisterLogSource(ILogger logSource) => logSource.LogEvent += OnLog;

        public bool OnLog(ILogger source, AbstractLogArgs args)
        {
            bool written = false;
            try
            {
                debugWriter.WriteLine($"[From : {source.Source,10}]");
                debugWriter.WriteLine($"[At   : {DateTime.Now,10}]");
                debugWriter.WriteLine(args.ToString());
                debugWriter.WriteLine();
                if (args.Level != LogLevel.Debug)
                {
                    logWriter.WriteLine(args.ToString());
                    logWriter.WriteLine();
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
            if (!disposedValue)
            {
                logOutput.Dispose();
                logWriter.Dispose();
                debugOutput.Dispose();
                debugWriter.Dispose();
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
