using DataCustoms.Logging;
using System.Reflection;

namespace DataCustoms
{
    public class DataMain
    {
        public static Mercury Logger { get; private set; } = new(Assembly.GetEntryAssembly()?.GetName().Name ?? "Main", false);
    }
}
