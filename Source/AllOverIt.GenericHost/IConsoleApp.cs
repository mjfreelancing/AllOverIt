namespace AllOverIt.GenericHost
{
    /// <summary>Represents a console application that can be hosted by .NET's hosting service.</summary>
    public interface IConsoleApp : IGenericApp
    {
        /// <summary>The exit code returned if <see cref="ExitCode"/> is not set (for example, the application
        /// completed without setting one, or was cancelled before it could).</summary>
        int DefaultExitCode { get; }

        /// <summary>The exit code returned if there is an unhandled exception.</summary>
        int UnhandedErrorExitCode { get; }

        /// <summary>The exit code returned if the user cancelled (Ctrl+C / Ctrl+Break or SIGINT) and the application
        /// did not set <see cref="ExitCode"/>.</summary>
        int CancelledExitCode { get; }

        /// <summary>The application's exit code.</summary>
        int? ExitCode { get; }

        /// <summary>
        /// Indicates if the user requested shutdown via Ctrl+C / Ctrl+Break (Windows) or SIGINT (Unix).
        /// It is not set on SIGTERM.
        /// </summary>
        bool UserCancelled { get; }
    }
}