using System.Diagnostics;

namespace DataCustoms.Logging
{
    public partial class Mercury : ILogger
    {
        public string Source { get; init; }
        public bool IsDiscrete { get; init; }

        internal Mercury() : this("DataCustoms", true) { }

        public Mercury(string owner, bool discrete)
        {
            Source = owner;
            IsDiscrete = discrete;
            LogWriter.Main.RegisterLogSource(this);
        }

        public event LogEvent LogEvent;

        private static string CallerPath(string filePath) => Path.GetFileNameWithoutExtension(filePath);
        public void Log(LogLevel level, string callerName, string callerClass, params object[] data)
        {

            LogEvent?.Invoke(this, new LogArgs(this, level, callerName, callerClass, data));
        }

        public void LogInfo(params object[] data)
        {
            StackFrame frame = new(1, true);
            string memberName = frame.GetMethod()?.Name ?? string.Empty;
            string filePath = frame.GetFileName() ?? string.Empty;
            Log(LogLevel.Info, memberName, CallerPath(filePath), data);
        }

        public void LogWarn(params object[] data)
        {
            StackFrame frame = new(1, true);
            string memberName = frame.GetMethod()?.Name ?? string.Empty;
            string filePath = frame.GetFileName() ?? string.Empty;
            Log(LogLevel.Debug, memberName, CallerPath(filePath), data);
        }

        public void LogError(params object[] data)
        {
            StackFrame frame = new(1, true);
            string memberName = frame.GetMethod()?.Name ?? string.Empty;
            string filePath = frame.GetFileName() ?? string.Empty;
            Log(LogLevel.Error, memberName, CallerPath(filePath), data);
        }

        public void LogCatch<TException>(TException exception) where TException : Exception
        {
            StackFrame frame = new(1, true);
            string memberName = frame.GetMethod()?.Name ?? string.Empty;
            string filePath = frame.GetFileName() ?? string.Empty;
            Log(LogLevel.Error, memberName, CallerPath(filePath), exception);
        }

        public void LogExcept<TException>(TException exception) where TException : Exception
        {
            StackFrame frame = new(1, true);
            string memberName = frame.GetMethod()?.Name ?? string.Empty;
            string filePath = frame.GetFileName() ?? string.Empty;
            Log(LogLevel.Error, memberName, CallerPath(filePath), exception);
            throw exception;
        }
    }
}
