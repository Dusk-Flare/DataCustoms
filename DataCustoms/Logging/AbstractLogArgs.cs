namespace DataCustoms.Logging
{
    public abstract class AbstractLogArgs(ILogger source, LogLevel level, params object[] data)
    {
        public object[] Data = data;
        public LogLevel Level = level;
        public ILogger Source = source;

        public abstract override string ToString();
    }
}
