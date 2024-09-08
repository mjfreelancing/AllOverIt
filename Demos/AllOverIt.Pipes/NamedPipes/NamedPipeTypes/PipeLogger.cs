using AllOverIt.Logging;

namespace NamedPipeTypes
{
    public static class PipeLogger
    {
        private static readonly ColorConsoleLogger Logger = new();
        private static readonly Lock _syncRoot = new();

        public static void Append(ConsoleColor color, string message)
        {
            lock (_syncRoot)
            {
                Logger.WriteLine(color, message);
            }
        }
    }
}