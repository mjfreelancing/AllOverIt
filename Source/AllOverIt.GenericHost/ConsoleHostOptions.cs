namespace AllOverIt.GenericHost
{
    /// <summary>Provides configuration options for a hosted console application.</summary>
    public sealed class ConsoleHostOptions
    {
        /// <summary>
        /// The maximum amount of time to wait for the console application to finish unwinding when the
        /// host is stopping, before its exit code is read.
        /// </summary>
        /// <remarks>
        /// If the console application does not finish unwinding within this time (for example because it
        /// cannot be cancelled), the host falls back to the exit-code fallback - the application's default
        /// exit code, or its cancelled exit code when the user cancelled. The default is 5 seconds.
        /// </remarks>
        public TimeSpan ShutdownWaitTimeout { get; set; } = TimeSpan.FromSeconds(5);
    }
}
