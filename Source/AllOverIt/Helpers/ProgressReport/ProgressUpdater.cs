using System;

namespace AllOverIt.Helpers.ProgressReport
{

    public delegate string GetProgressText(int progress);

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
        public static Action<GetProgressText> Create(int total, int incrementToReport, Action<ProgressState> notifier)
        {
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

                if (roundedProgress != previousProgress && (roundedProgress == total || roundedProgress >= previousProgress + incrementToReport))
                {
                    previousProgress = roundedProgress;
                    return roundedProgress;
                }

                return -1; // A sentinal to indicate no change in progress
            };
        }
    }
}
