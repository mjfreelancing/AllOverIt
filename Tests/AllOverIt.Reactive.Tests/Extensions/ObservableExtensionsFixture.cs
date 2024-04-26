using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FluentAssertions;
using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

using ObservableExtensions = AllOverIt.Reactive.Extensions.ObservableExtensions;

namespace AllOverIt.Reactive.Tests.Extensions
{
    public class ObservableExtensionsFixture : FixtureBase
    {
        public class WaitUntil : ObservableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Observable_Null()
            {
                Invoking(() =>
                    {
                        ObservableExtensions.WaitUntil((IObservable<int>) null, _ => true);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("observable");
            }

            [Fact]
            public void Should_Throw_When_Predicate_Null()
            {
                Invoking(() =>
                    {
                        ObservableExtensions.WaitUntil(this.CreateStub<IObservable<int>>(), null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("predicate");
            }

            [Fact]
            public void Should_Return_Expected_Result_No_Action()
            {
                var expected = GetWithinRange(7, 9);
                var actual = 0;
                var subject = new Subject<int>();

                ObservableExtensions
                    .WaitUntil(subject, value => value == expected)
                    .Subscribe(value =>
                    {
                        actual = value;
                    });

                for (var i = expected - 1; i <= expected + 1; i++)
                {
                    subject.OnNext(i);
                }

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Return_Expected_Result_Action()
            {
                var expected = GetWithinRange(7, 9);
                var multiplier = GetWithinRange(3, 5);
                var actual = 0;
                var subject = new Subject<int>();

                ObservableExtensions
                    .WaitUntil<int>(subject, value => value == expected, value => value * multiplier)
                    .Subscribe(value =>
                    {
                        actual = value;
                    });

                for (var i = expected - 1; i <= expected + 1; i++)
                {
                    subject.OnNext(i);
                }

                actual.Should().Be(expected * multiplier);
            }
        }

        public class WaitUntil_Project_Result_Type : ObservableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Observable_Null()
            {
                Invoking(() =>
                    {
                        ObservableExtensions.WaitUntil<int, int>(null, _ => true, value => value);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("observable");
            }

            [Fact]
            public void Should_Throw_When_Predicate_Null()
            {
                Invoking(() =>
                    {
                        ObservableExtensions.WaitUntil<int, int>(this.CreateStub<IObservable<int>>(), null,
                            value => value);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("predicate");
            }

            [Fact]
            public void Should_Return_Expected_Result_Action()
            {
                var expected = GetWithinRange(7, 9);
                var multiplier = GetWithinRange(3, 5);
                var actual = string.Empty;
                var subject = new Subject<int>();

                ObservableExtensions
                    .WaitUntil(subject, value => value == expected, result => $"{result * multiplier}")
                    .Subscribe(value =>
                    {
                        actual = value;
                    });

                for (var i = expected - 1; i <= expected + 1; i++)
                {
                    subject.OnNext(i);
                }

                actual.Should().Be($"{expected * multiplier}");
            }
        }

        public class WaitUntilAsync : ObservableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Observable_Null()
            {
                await Invoking(async () =>
                    {
                        _ = await ObservableExtensions.WaitUntilAsync((IObservable<int>) null, _ => true);
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("observable");
            }

            [Fact]
            public async Task Should_Throw_When_Predicate_Null()
            {
                await Invoking(async () =>
                    {
                        _ = await ObservableExtensions.WaitUntilAsync(this.CreateStub<IObservable<int>>(), null);
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("predicate");
            }

            [Fact]
            public async Task Should_Return_Expected_Result_No_Action()
            {
                var expected = GetWithinRange(7, 9);
                var subject = new Subject<int>();

                var observable = ObservableExtensions.WaitUntilAsync(
                    subject,
                    value => value == expected);

                for (var i = expected - 1; i <= expected + 1; i++)
                {
                    subject.OnNext(i);
                }

                var actual = await observable;

                actual.Should().Be(expected);
            }

            [Fact]
            public async Task Should_Return_Expected_Result_Action()
            {
                var expected = GetWithinRange(7, 9);
                var multiplier = GetWithinRange(3, 5);
                var subject = new Subject<int>();

                var observable = ObservableExtensions
                    .WaitUntilAsync<int>(
                        subject,
                        value => value == expected,
                        value => Task.FromResult(value * multiplier));

                for (var i = expected - 1; i <= expected + 1; i++)
                {
                    subject.OnNext(i);
                }

                var actual = await observable;

                actual.Should().Be(expected * multiplier);
            }
        }

        public class WaitUntilAsync_Project_Result_Type : ObservableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Observable_Null()
            {
                await Invoking(async () =>
                    {
                        _ = await ObservableExtensions.WaitUntilAsync<int, int>(
                            null,
                            _ => true,
                            Task.FromResult);
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("observable");
            }

            [Fact]
            public async Task Should_Throw_When_Predicate_Null()
            {
                await Invoking(async () =>
                    {
                        _ = await ObservableExtensions.WaitUntilAsync<int, int>(
                            this.CreateStub<IObservable<int>>(),
                            null,
                            Task.FromResult);
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("predicate");
            }

            [Fact]
            public async Task Should_Return_Expected_Result_Action()
            {
                var expected = GetWithinRange(7, 9);
                var multiplier = GetWithinRange(3, 5);
                var subject = new Subject<int>();

                var observable = ObservableExtensions
                    .WaitUntilAsync(
                        subject,
                        value => value == expected,
                        result => Task.FromResult($"{result * multiplier}"));

                for (var i = expected - 1; i <= expected + 1; i++)
                {
                    subject.OnNext(i);
                }

                var actual = await observable;

                actual.Should().Be($"{expected * multiplier}");
            }

            [Fact]
            public async Task Should_Return_Expected_Exception()
            {
                var expected = GetWithinRange(7, 9);
                var multiplier = GetWithinRange(3, 5);
                var subject = new Subject<int>();

                var observable = ObservableExtensions
                    .WaitUntilAsync(
                        subject,
                        value => value == expected,
                        result => throw new Exception($"{result * multiplier}"));

                for (var i = expected - 1; i <= expected + 1; i++)
                {
                    subject.OnNext(i);
                }

                await Invoking(async () =>
                    {
                        _ = await observable;
                    })
                    .Should()
                    .ThrowAsync<Exception>()
                    .WithMessage($"{expected * multiplier}");
            }
        }
    }
}