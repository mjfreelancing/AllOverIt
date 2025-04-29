using AllOverIt.Logging;

namespace NamedPipeTypes
{
    public static class PipeLogger
    {
        // TODO: Update when NET 8 is no longer supported
#if NET9_0_OR_GREATER
        private static readonly Lock SyncRoot = new();
#else
        private static readonly object SyncRoot = new();
#endif

        private static readonly ColorConsoleLogger Logger = new();

        public static void Append(ConsoleColor color, string message)
        {
            lock (SyncRoot)
            {
                Logger.WriteLine(color, message);
            }
        }
    }
}