using AllOverIt.Logging;

namespace NamedPipeTypes
{
    public static class PipeLogger
    {
        // TODO: Update when NET 8 is no longer supported
#if NET9_0_OR_GREATER
        private static readonly Lock _syncRoot = new();
#else
        private static readonly object _syncRoot = new();
#endif

        private static readonly ColorConsoleLogger _logger = new();

        public static void Append(ConsoleColor color, string message)
        {
            lock (_syncRoot)
            {
                _logger.WriteLine(color, message);
            }
        }
    }
}