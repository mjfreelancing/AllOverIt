namespace AllOverIt.GenericHost
{
    public interface IConsoleApp : IGenericApp
    {
        // This exit code is returned if ExitCode is not set (which can occur if cancelled via Ctrl+C/SIGTERM)
        int DefaultExitCode { get; }

        // This exit code is returned if there is an unhandled exception
        int UnhandedErrorExitCode { get; }

        int? ExitCode { get; }
    }
}