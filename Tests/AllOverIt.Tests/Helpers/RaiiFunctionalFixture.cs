using AllOverIt.Fixture;
using AllOverIt.Helpers;
using FluentAssertions;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Helpers
{
    public class RaiiFunctionalFixture : AoiFixtureBase
    {
        private class Logger
        {
            private class ProfilerContext
            {
                public Stopwatch Stopwatch { get; }
                public string Title { get; }

                public ProfilerContext(string title)
                {
                    Stopwatch = Stopwatch.StartNew();
                    Title = title;
                }
            }

            // use the to emulate logging to some external output
            private readonly Action<string> _action;

            public Logger(Action<string> action)
            {
                _action = action;
            }

            public void LogMessage(string message)
            {
                _action.Invoke(message); ;
            }

            public IDisposable GetProfiler(string title)
            {
                return new Raii<ProfilerContext>(() => new ProfilerContext(title), context => StopProfiling(context));
            }

            private void StopProfiling(ProfilerContext context)
            {
                var message = $"{context.Title} took {context.Stopwatch.ElapsedMilliseconds}ms";

                LogMessage(message);
            }
        }

        [Fact]
        public async Task Should_Log_Profile_Period()
        {
            var title = Create<string>();

            var actual = string.Empty;

            var logger = new Logger(message => actual = message);

            using (logger.GetProfiler(title))
            {
                await Task.Delay(100).ConfigureAwait(false);
            }

            actual.Should().StartWith($"{title} took ");
            actual.Should().EndWith("ms");
        }
    }
}