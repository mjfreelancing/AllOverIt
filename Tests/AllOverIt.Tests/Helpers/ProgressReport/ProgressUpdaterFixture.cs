using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Helpers.ProgressReport;
using FluentAssertions;
using System;
using System.Collections.Generic;

namespace AllOverIt.Tests.Helpers.ProgressReport
{
    public class ProgressUpdaterFixture : FixtureBase
    {
        public class Create : ProgressUpdaterFixture
        {
            [Fact]
            public void Should_Throw_When_Notifier_Null()
            {
                Invoking(() =>
                {
                    _ = ProgressUpdater.Create(Create<int>(), 1, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("notifier");
            }

            [Theory]
            [InlineData(-1, false)]
            [InlineData(0, false)]
            [InlineData(1, true)]
            public void Should_Have_A_Valid_Total(int total, bool expected)
            {
                var assertion = Invoking(() =>
                {
                    _ = ProgressUpdater.Create(total, 1, _ => { });
                })
                .Should();

                if (expected)
                {
                    _ = assertion.NotThrow();
                }
                else
                {
                    _ = assertion
                        .Throw<ArgumentOutOfRangeException>()
                        .WithMessage("The total count must be greater than zero. (Parameter 'total')");
                }
            }

            [Theory]
            [InlineData(-1, false)]
            [InlineData(0, false)]
            [InlineData(1, true)]
            [InlineData(100, true)]
            [InlineData(101, false)]
            public void Should_Have_A_Valid_Increment(int increment, bool expected)
            {
                var assertion = Invoking(() =>
                {
                    _ = ProgressUpdater.Create(Create<int>(), increment, _ => { });
                })
                .Should();

                if (expected)
                {
                    _ = assertion.NotThrow();
                }
                else
                {
                    _ = assertion
                        .Throw<ArgumentOutOfRangeException>()
                        .WithMessage("The reporting increment must have a value between 1 and 100. (Parameter 'incrementToReport')");
                }
            }

            [Theory]
            [InlineData(1, 1, new[] { 100 })]
            [InlineData(2, 1, new[] { 50, 100 })]
            [InlineData(3, 1, new[] { 33, 66, 100 })]
            [InlineData(5, 3, new[] { 20, 40, 60, 80, 100 })]
            [InlineData(10, 11, new[] { 20, 40, 60, 80, 100 })]
            [InlineData(12, 30, new[] { 33, 66, 100 })]
            [InlineData(15, 9, new[] { 13, 26, 40, 53, 66, 80, 93, 100 })]
            [InlineData(100, 15, new[] { 15, 30, 45, 60, 75, 90, 100 })]
            [InlineData(100, 17, new[] { 17, 34, 51, 68, 85, 100 })]
            [InlineData(100, 20, new[] { 20, 40, 60, 80, 100 })]
            public void Should_Report_Increments(int total, int increment, int[] expected)
            {
                var actual = new List<int>();

                var updater = ProgressUpdater.Create(total, increment, state =>
                {
                    actual.Add(state.Progress);
                });

                for (var i = 0; i < total; i++)
                {
                    updater.Invoke(_ => string.Empty);
                }

                actual.Should().BeEquivalentTo(expected);
            }
        }
    }
}
