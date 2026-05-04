using AllOverIt.Fixture;
using Shouldly;
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
                _context.LogLevels.ShouldBeEquivalentTo(_context.Select(item => item.LogLevel).ToArray());
            }
        }

        public class Metadata : MethodCallContextFixture
        {
            [Fact]
            public void Should_Return_Metadata()
            {
                _context.Metadata.ShouldBeEquivalentTo(_context.Select(item => item.Metadata).ToArray());
            }
        }

        public class Exceptions : MethodCallContextFixture
        {
            [Fact]
            public void Should_Return_Exceptions()
            {
                _context.Exceptions.ShouldBeEquivalentTo(_context.Select(item => item.Exception).ToArray());
            }
        }
    }
}