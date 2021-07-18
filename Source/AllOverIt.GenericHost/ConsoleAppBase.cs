namespace AllOverIt.GenericHost
{
    public abstract class ConsoleAppBase : GenericAppBase, IConsoleApp
    {
        public int DefaultExitCode { get; protected set; } = 0;
        public int UnhandedErrorExitCode { get; protected set; } = -1;

        public int? ExitCode { get; protected set; }
    }
}