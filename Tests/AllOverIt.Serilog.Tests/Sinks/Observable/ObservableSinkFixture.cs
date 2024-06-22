using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Serilog.Extensions;
using AllOverIt.Serilog.Sinks.Observable;
using FluentAssertions;
using Serilog;

namespace AllOverIt.Serilog.Tests.Sinks.Observable
{
    public class ObservableSinkFixture : FixtureBase
    {
        private readonly ObservableSink _observableSink = new ObservableSink();

        public class Count : ObservableSinkFixture
        {
            [Fact]
            public void Should_Have_Expected_Count()
            {
                _observableSink.Count.Should().Be(0);

                var subscription1 = _observableSink.Subscribe();
                _observableSink.Count.Should().Be(1);

                var subscription2 = _observableSink.Subscribe();
                _observableSink.Count.Should().Be(2);

                subscription2.Dispose();
                _observableSink.Count.Should().Be(1);

                subscription1.Dispose();
                _observableSink.Count.Should().Be(0);
            }
        }

        public class Subscribe : ObservableSinkFixture
        {
            [Fact]
            public void Should_Throw_When_Observer_Null()
            {
                Invoking(() =>
                {
                    _ = _observableSink.Subscribe(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("observer");
            }

            [Fact]
            public void Should_Receive_Message_When_Subscribed()
            {
                var message = Create<string>();
                var actual = string.Empty;

                using var logger = new LoggerConfiguration().WriteTo.Observable(_observableSink).CreateLogger();

                using var subscriber = _observableSink
                    .Subscribe(logEvent =>
                    {
                        actual = logEvent.MessageTemplate.Text;
                    });

                logger.Information(message);

                actual.Should().Be(message);
            }

            [Fact]
            public void Should_Not_Receive_Message_When_Disposed()
            {
                var message = Create<string>();
                var actual = string.Empty;

                using var logger = new LoggerConfiguration().WriteTo.Observable(_observableSink).CreateLogger();

                using var subscriber = _observableSink
                    .Subscribe(logEvent =>
                    {
                        actual = logEvent.MessageTemplate.Text;
                    });

                logger.Information(message);

                actual.Should().Be(message);

                subscriber.Dispose();

                actual = null;

                logger.Information(Create<string>());

                actual.Should().BeNull();
            }

            [Fact]
            public void Should_Throw_When_Sink_Already_Disposed()
            {
                _observableSink.Dispose();

                Invoking(() =>
                {
                    _ = _observableSink.Subscribe();
                })
                    .Should()
                    .Throw<ObjectDisposedException>();
            }
        }

        public class Disposal : ObservableSinkFixture
        {
            [Fact]
            public void Should_Have_Expected_Count()
            {
                var received1 = new List<string>();
                var received2 = new List<string>();

                using var logger = new LoggerConfiguration().WriteTo.Observable(_observableSink).CreateLogger();

                var subscription1 = _observableSink.Subscribe(logEvent => received1.Add(logEvent.MessageTemplate.Text));
                var subscription2 = _observableSink.Subscribe(logEvent => received2.Add(logEvent.MessageTemplate.Text));

                logger.Information(Create<string>());
                logger.Information(Create<string>());

                received1.Should().HaveCount(2);
                received2.Should().HaveCount(2);

                received1.Should().BeEquivalentTo(received2, options => options.WithStrictOrdering());

                subscription1.Dispose();
                subscription2.Dispose();

                logger.Information(Create<string>());
                logger.Information(Create<string>());

                received1.Should().HaveCount(2);
                received2.Should().HaveCount(2);
            }

            [Fact]
            public void Should_Notify_Completion()
            {
                var completed = false;

                // not 'using' since the subscriber would outlive the sink and throw an ObjectDisposedException
                var subscriber = _observableSink.Subscribe(onNext: logEvent => { }, onCompleted: () => completed = true);

                _observableSink.Dispose();

                completed.Should().BeTrue();
            }

            [Fact]
            public void Should_Throw_When_Outlive_Sink()
            {
                var subscriber = _observableSink.Subscribe();

                _observableSink.Dispose();

                Invoking(() =>
                {
                    subscriber.Dispose();
                })
                    .Should()
                    .Throw<ObjectDisposedException>()
                    .WithMessage("The observable sink has been disposed.");
            }

            [Fact]
            public void Should_Not_Throw_When_Already_Disposed()
            {
                Invoking(() =>
                {
                    _observableSink.Dispose();
                    _observableSink.Dispose();
                })
                    .Should()
                    .NotThrow();
            }
        }

        public class Emit : ObservableSinkFixture
        {
            [Fact]
            public void Should_Capture_Log_Events()
            {
                using var logger = new LoggerConfiguration().WriteTo.Observable(_observableSink).CreateLogger();

                var messages = CreateMany<string>(5).ToArray();
                var receivedCount = 0;

                using var subscriber = _observableSink
                    .Subscribe(logEvent =>
                    {
                        receivedCount++;
                    });

                foreach (var message in messages)
                {
                    logger.Information(message);
                }

                receivedCount.Should().Be(5);
            }

            [Fact]
            public void Should_Raise_OnError()
            {
                var errors = new List<Exception>();

                using var logger = new LoggerConfiguration().WriteTo.Observable(_observableSink).CreateLogger();

                using var subscriber1 = _observableSink
                    .Subscribe(
                        onNext: logEvent =>
                        {
                            throw new Exception("abc");
                        },
                        onCompleted: () => { },
                        onError: exception => { errors.Add(exception); });

                using var subscriber2 = _observableSink
                    .Subscribe(logEvent =>
                    {
                    });

                using var subscriber3 = _observableSink
                    .Subscribe(
                        onNext: logEvent =>
                        {
                            throw new Exception("def");
                        },
                        onCompleted: () => { },
                        onError: exception => { errors.Add(exception); });

                logger.Information(Create<string>());

                errors.Select(item => item.Message).Should().BeEquivalentTo(["abc", "def"]);
            }
        }
    }
}