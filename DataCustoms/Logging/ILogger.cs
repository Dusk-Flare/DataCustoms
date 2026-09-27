namespace DataCustoms.Logging
{
    public interface ILogger
    {
        public string Source { get; init; }
        public bool IsDiscrete { get; init; }
        public event LogEvent LogEvent;
        public static void Register(ILogger logger) => LogWriter.Main.RegisterLogSource(logger);
    }
}
