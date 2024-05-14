using AllOverIt.Fixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace AllOverIt.Logging.Testing.Tests
{
    public class MethodCallContextFixture : FixtureBase
    {
        private readonly MethodCallContext _context;

        public MethodCallContextFixture()
        {
            _context = new MethodCallContext();

            for (var i = 0; i < 5; i++)
            {
                var item = new MethodCallContext.Item
                {
                    LogLevel = Create<LogLevel>(),
                    Metadata = CreateMany<KeyValuePair<string, string>>().ToDictionary(kvp => kvp.Key, kvp => (object) kvp.Value),
                    Exception = new Exception(Create<string>())
                };

                _context.Add(item);
            }
        }

        public class LogLevels : MethodCallContextFixture
        {
            [Fact]
            public void Should_Return_LogLevels()
            {
                _context.LogLevels.Should().BeEquivalentTo(_context.Select(item => item.LogLevel));
            }
        }

        public class Metadata : MethodCallContextFixture
        {
            [Fact]
            public void Should_Return_Metadata()
            {
                _context.Metadata.Should().BeEquivalentTo(_context.Select(item => item.Metadata));
            }
        }

        public class Exceptions : MethodCallContextFixture
        {
            [Fact]
            public void Should_Return_Exceptions()
            {
                _context.Exceptions.Should().BeEquivalentTo(_context.Select(item => item.Exception));
            }
        }
    }
}