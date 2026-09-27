using System.Text;

namespace DataCustoms.Logging
{
    public class LogArgs(ILogger source, LogLevel level, string callerName, string callerClass, params object[] data) : AbstractLogArgs(source, level, data)
    {
        public override string ToString()
        {
            StringBuilder builder = new();
            builder.Append($"[{Level,-10} : {Source.Source,10}]");
            foreach (object dataObj in Data)
            {
                object data = dataObj;
                string prefix = $"\n[At {callerName,-7} : {callerClass,10}]: ";
                if (data is Exception ex) data = ex.StackTrace ?? string.Empty;
                string indent = "\n".PadRight(prefix.Length);
                string info = data?.ToString() ?? string.Empty;
                string[] lines = info.Split('\n')
                .SkipWhile(string.IsNullOrEmpty)
                .Reverse()
                .SkipWhile(string.IsNullOrEmpty)
                .Reverse()
                .ToArray();
                foreach (string line in lines)
                {
                    builder.Append(prefix).Append(line);
                    prefix = indent;
                }
            }
            return builder.ToString();
        }
    }
}
