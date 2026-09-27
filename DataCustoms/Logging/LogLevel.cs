namespace DataCustoms.Logging
{
    [Flags]
    public enum LogLevel
    {
        None = 0,
        Info = 1 << 0,
        Debug = 1 << 1,
        Error = 1 << 2
    }
}
