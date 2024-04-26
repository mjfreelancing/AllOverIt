using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Tests.Extensions
{
    public class ExceptionExtensionsFixture : FixtureBase
    {
        public class Walk : ExceptionExtensionsFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Exception_Null()
            {
                Invoking(() =>
                {
                    Exception exception = null;

                    ExceptionExtensions.Walk(exception, ex => { });
                })
                .Should()
                .NotThrow();
            }

            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Invoking(() =>
                {
                    var exception = new Exception();

                    ExceptionExtensions.Walk(exception, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("onException");
            }

            [Fact]
            public void Should_Only_Process_Root_Exception()
            {
                var exception = CreateExceptionWithDepth(10);       // 0 .. 10

                var messages = new List<string>();
                Action<Exception> handler = ex => { messages.Add(ex.Message); };

                ExceptionExtensions.Walk(exception, handler, 0);

                messages.Should().BeEquivalentTo(new[] { "0" });
            }

            [Fact]
            public void Should_Ignore_Recursion_Depth()
            {
                var exception = CreateExceptionWithDepth(10);       // 0 .. 10

                var messages = new List<string>();
                Action<Exception> handler = ex => { messages.Add(ex.Message); };

                ExceptionExtensions.Walk(exception, handler, -1);

                var expected = Enumerable.Range(0, 11).Select(item => $"{item}");

                messages.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Not_Exceed_Recursion_Depth()
            {
                var maxDepth = GetWithinRange(3, 8);                // 0 .. n
                var exception = CreateExceptionWithDepth(10);       // 0 .. 10

                var messages = new List<string>();
                Action<Exception> handler = ex => { messages.Add(ex.Message); };

                ExceptionExtensions.Walk(exception, handler, maxDepth);

                var expected = Enumerable.Range(0, maxDepth + 1).Select(item => $"{item}");

                messages.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Walk_Aggregate_Exception_Ignoring_Depth()
            {
                var exception = new AggregateException("aggregate", new[]
                {
                    CreateExceptionWithDepth(3),
                    CreateAggregateException(5),
                    CreateAggregateException(4),
                    CreateExceptionWithDepth(3)
                });

                var messages = new List<string>();
                Action<Exception> handler = ex => { messages.Add(ex.Message); };

                ExceptionExtensions.Walk(exception, handler, -1);

                var expected = new[] {
                    // Outer aggregate
                    "aggregate (0) (aggregate (1) (2) (3) (4) (5)) (aggregate (1) (2) (3) (4)) (0)",

                    // First inner, down to all levels
                    "0",
                    "1",
                    "2",
                    "3",

                    // Second inner, an aggregate with all exceptions at the first level
                    "aggregate (1) (2) (3) (4) (5)",
                    "1", "2", "3", "4", "5",

                    // third inner, an aggregate with all exceptions at the first level
                    "aggregate (1) (2) (3) (4)",
                    "1", "2", "3", "4",

                    // fourth inner, down to all levels
                    "0",
                    "1",
                    "2",
                    "3"
                };

                messages.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Walk_Aggregate_Exception_With_Depth()
            {
                var exception = new AggregateException("aggregate", new[]
                {
                    CreateExceptionWithDepth(3),
                    CreateAggregateException(5),
                    CreateAggregateException(4),
                    CreateExceptionWithDepth(3)
                });

                var messages = new List<string>();
                Action<Exception> handler = ex => { messages.Add(ex.Message); };

                ExceptionExtensions.Walk(exception, handler, 2);

                var expected = new[] {
                    // Outer aggregate
                    "aggregate (0) (aggregate (1) (2) (3) (4) (5)) (aggregate (1) (2) (3) (4)) (0)",

                    // First inner, down to 2 levels
                    "0",
                    "1",

                    // Second inner, an aggregate with all exceptions at the first level
                    "aggregate (1) (2) (3) (4) (5)",
                    "1", "2", "3", "4", "5",

                    // third inner, an aggregate with all exceptions at the first level
                    "aggregate (1) (2) (3) (4)",
                    "1", "2", "3", "4",

                    // fourth inner, down to 2 levels
                    "0",
                    "1"
                };

                messages.Should().BeEquivalentTo(expected);
            }
        }

        private static Exception CreateExceptionWithDepth(int depth)
        {
            var exception = new Exception($"{depth}");

            while (depth > 0)
            {
                depth--;
                exception = new Exception($"{depth}", exception);
            }

            return exception;
        }

        private static Exception CreateAggregateException(int innerCount)
        {
            var exceptions = Enumerable.Range(1, innerCount).Select(item => new Exception($"{item}"));
            return new AggregateException("aggregate", exceptions);
        }
    }
}
