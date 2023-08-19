using AllOverIt.Assertion;
using System;

namespace AllOverIt.Helpers.ProgressReport
{
    /// <summary>Defines a delegate type that accepts a progress value and returns a string to represent this value.</summary>
    /// <param name="progress">The progress value. Will have a value between 0 and 100, inclusively.</param>
    /// <returns>A string representation of the <paramref name="progress"/> value.</returns>
    public delegate string GetProgressText(int progress);

    /// <summary>A factory class used to create an <c>Action&lt;GetProgressText></c>that when invoked will increment, and notify, the progress of an operation.</summary>
    public static class ProgressUpdater
    {
        /// <summary>Creates a method that is expected to be called a given number of times, as specified by the <paramref name="total"/>
        /// argument. Each time the method is called, the current number of invocations is calculated as a percentage of <paramref name="total"/>.
        /// If the calculated progress is an increment of <paramref name="incrementToReport"/> or has a value of 100 then the <paramref name="notifier"/>
        /// will be invoked. This callback is only ever called once per unique increment.</summary>
        /// <param name="total">The expected total number of times the returned Action will be invoked.</param>
        /// <param name="incrementToReport">Invokes the <paramref name="notifier"/> at (percentage) increments specified by this argument.</param>
        /// <param name="notifier">The callback to be invoked as progress occurs. The input argument is an optional callback to get the desired status
        /// text. Pass <see langword="null"/> to use the most recent status text.</param>
        /// <returns>An action that should be invoked up to <paramref name="total"/> times. This action will invoke <paramref name="notifier"/> at
        /// percentage increments as specified by <paramref name="incrementToReport"/>.</returns>
        /// <remarks>No validation is performed on the calculated progress. If the returned action is invoked more than <paramref name="total"/> times
        /// then a value greater than 100 may be returned (depending on the value of <paramref name="incrementToReport"/>).</remarks>
        public static Action<GetProgressText> Create(int total, int incrementToReport, Action<ProgressState> notifier)
        {
            _ = notifier.WhenNotNull(nameof(notifier));

            Throw<ArgumentOutOfRangeException>.WhenNot(total > 0, nameof(total), "The total count must be greater than zero.");
            Throw<ArgumentOutOfRangeException>.WhenNot(incrementToReport > 0 && incrementToReport <= 100, nameof(incrementToReport), "The reporting increment must have a value between 1 and 100.");

            var calculateProgress = GetProgressCalculator(total, incrementToReport);
            var progressText = string.Empty;

            return getText =>
            {
                var progress = calculateProgress.Invoke();

                if (progress != -1)
                {
                    var text = getText?.Invoke(progress);

                    progressText = text ?? progressText ?? string.Empty;

                    var state = new ProgressState
                    {
                        Progress = progress,
                        Text = text
                    };

                    notifier.Invoke(state);
                }
            };
        }

        private static Func<int> GetProgressCalculator(int total, int incrementToReport)
        {
            var currentProgress = 0;
            var previousProgress = -1;

            return () =>
            {
                var progress = ++currentProgress * 100.0d / total;
                var roundedProgress = (int) progress;

                var nextReportableProgress = previousProgress == -1
                    ? incrementToReport
                    : previousProgress + incrementToReport;

                if (roundedProgress != previousProgress && (currentProgress == total || roundedProgress >= nextReportableProgress))
                {
                    previousProgress = roundedProgress;

                    return roundedProgress;
                }

                return -1; // A sentinal to indicate no change in progress
            };
        }
    }
}
