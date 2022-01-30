namespace AllOverIt.GenericHost
{
    /// <summary>Represents a console application that can be hosted by .NET's hosting service.</summary>
    public interface IConsoleApp : IGenericApp
    {
        /// <summary>The exit code returned if <see cref="ExitCode"/> is not set (which can occur if cancelled via Ctrl+C/SIGTERM).</summary>
        int DefaultExitCode { get; }

        /// <summary>The exit code returned if there is an unhandled exception.</summary>
        int UnhandedErrorExitCode { get; }

        /// <summary>The application's exit code.</summary>
        int? ExitCode { get; }
    }
}