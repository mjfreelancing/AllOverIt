using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Helpers;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Helpers
{
    public class ExceptionAggregatorFixture : FixtureBase
    {
        private readonly ExceptionAggregator _aggregator = new();

        public class Exceptions : ExceptionAggregatorFixture
        {
            [Fact]
            public void Should_Have_Empty_Exceptions()
            {
                _aggregator.Exceptions.Should().BeEmpty();
            }
        }

        public class AddException : ExceptionAggregatorFixture
        {
            [Fact]
            public void Should_Throw_When_Exception_Null()
            {
                Invoking(() =>
                {
                    _aggregator.AddException(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("exception");
            }

            [Fact]
            public void Should_Add_Exceptions()
            {
                var exception1 = new Exception();
                var exception2 = new Exception();

                _aggregator.AddException(exception1);
                _aggregator.AddException(exception2);

                _aggregator.Exceptions.Should().HaveCount(2);

                _aggregator.Exceptions.ElementAt(0).Should().BeSameAs(exception1);
                _aggregator.Exceptions.ElementAt(1).Should().BeSameAs(exception2);
            }
        }

        public class ThrowIfAnyExceptions : ExceptionAggregatorFixture
        {
            [Fact]
            public void Should_Not_Throw_When_No_Exceptions()
            {
                Invoking(() =>
                {
                    _aggregator.ThrowIfAnyExceptions();
                })
                .Should()
                .NotThrow();
            }

            [Fact]
            public void Should_Throw_Flattened_False()
            {
                var exception1 = new Exception();
                var exception2 = new Exception();
                var exception3 = new AggregateException(exception1, exception2);

                _aggregator.AddException(exception1);
                _aggregator.AddException(exception2);
                _aggregator.AddException(exception3);

                try
                {
                    _aggregator.ThrowIfAnyExceptions(false);
                }
                catch (AggregateException exception)
                {
                    exception.InnerExceptions.ElementAt(0).Should().BeSameAs(exception1);
                    exception.InnerExceptions.ElementAt(1).Should().BeSameAs(exception2);
                    exception.InnerExceptions.ElementAt(2).Should().BeSameAs(exception3);
                }
            }

            [Fact]
            public void Should_Throw_Flattened_True()
            {
                var exception1 = new Exception();
                var exception2 = new Exception();
                var exception3 = new AggregateException(exception1, exception2);

                _aggregator.AddException(exception1);
                _aggregator.AddException(exception2);
                _aggregator.AddException(exception3);

                try
                {
                    _aggregator.ThrowIfAnyExceptions(true);
                }
                catch (AggregateException exception)
                {
                    exception.InnerExceptions.ElementAt(0).Should().BeSameAs(exception1);
                    exception.InnerExceptions.ElementAt(1).Should().BeSameAs(exception2);
                    exception.InnerExceptions.ElementAt(2).Should().BeSameAs(exception1);
                    exception.InnerExceptions.ElementAt(3).Should().BeSameAs(exception2);
                }
            }
        }
    }
}
