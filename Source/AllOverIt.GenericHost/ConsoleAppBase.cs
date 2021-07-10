namespace AllOverIt.GenericHost
{
    public abstract class ConsoleAppBase : GenericAppBase, IConsoleApp
    {
        public int? ExitCode { get; protected set; }
    }
}