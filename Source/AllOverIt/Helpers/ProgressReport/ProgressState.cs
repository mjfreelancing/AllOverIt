namespace AllOverIt.Helpers.ProgressReport
{
    /// <summary>Provides progress state information during notification via the action returned from
    /// <see cref="ProgressUpdater.Create(int, int, System.Action{ProgressState})"/>.</summary>
    public sealed class ProgressState
    {
        /// <summary>The current progress, as a percentage.</summary>
        public int Progress { get; init; }

        /// <summary>The textual representation of the current progress.</summary>
        public required string Text { get; init; }
    }
}
