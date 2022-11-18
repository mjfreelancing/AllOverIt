using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.ResourceInitialization;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Patterns.ResourceInitialization
{
    public class RaiiAsyncFixture : FixtureBase
    {
        public class RaiiAsync_NonType : RaiiFixture
        {
            public class Constructor : Raii_NonType
            {
                [Fact]
                public void Should_Throw_When_Initialize_Null()
                {
                    Invoking(() =>
                        {
                            _ = new RaiiAsync(null, () => Task.CompletedTask);
                        })
                        .Should()
                        .Throw<ArgumentNullException>()
                        .WithNamedMessageWhenNull("initialize");
                }

                [Fact]
                public void Should_Throw_When_Cleanup_Null()
                {
                    Invoking(() =>
                        {
                            _ = new RaiiAsync(() => { }, null);
                        })
                        .Should()
                        .Throw<ArgumentNullException>()
                        .WithNamedMessageWhenNull("cleanup");
                }

                [Fact]
                public async Task Should_Call_Initialize()
                {
                    var initialized = false;

                    void Initialize()
                    {
                        initialized = true;
                    }

                    await using (new RaiiAsync(Initialize, () => Task.CompletedTask))
                    {
                        initialized.Should().BeTrue();
                    }
                }

                [Fact]
                public async Task Should_Not_Call_Cleanup()
                {
                    var cleanedUp = false;

                    Task Cleanup()
                    {
                        cleanedUp = true;
                        return Task.CompletedTask;
                    }

                    await using (new RaiiAsync(() => { }, Cleanup))
                    {
                        cleanedUp.Should().BeFalse();
                    }
                }

                [Fact]
                public async Task Should_Call_Cleanup()
                {
                    var cleanedUp = false;

                    Task Cleanup()
                    {
                        cleanedUp = true;
                        return Task.CompletedTask;
                    }

                    await using (new RaiiAsync(() => { }, Cleanup))
                    {
                    }

                    cleanedUp.Should().BeTrue();
                }
            }

            public class Disposal : Raii_NonType
            {
                [Fact]
                public async Task Should_Dispose()
                {
                    var cleanedUp = false;

                    Task Cleanup()
                    {
                        cleanedUp = true;
                        return Task.CompletedTask;
                    }

                    await using (new RaiiAsync(() => { }, Cleanup))
                    {
                    }

                    cleanedUp.Should().BeTrue();
                }
            }
        }

        public class RaiiAsync_Type : RaiiFixture
        {
            public class Constructor : RaiiAsync_Type
            {
                [Fact]
                public void Should_Throw_When_Initialize_Null()
                {
                    Invoking(() =>
                        {
                            _ = new RaiiAsync<int>(null, _ => Task.CompletedTask);
                        })
                        .Should()
                        .Throw<ArgumentNullException>()
                        .WithNamedMessageWhenNull("initialize");
                }

                [Fact]
                public void Should_Throw_When_Cleanup_Null()
                {
                    var value = Create<int>();

                    Invoking(() =>
                        {
                            _ = new RaiiAsync<int>(() => value, null);
                        })
                        .Should()
                        .Throw<ArgumentNullException>()
                        .WithNamedMessageWhenNull("cleanup");
                }

                [Fact]
                public async Task Should_Call_Initialize()
                {
                    var value = Create<int>();
                    var initialized = -value;

                    int Initialize()
                    {
                        initialized = value;
                        return value;
                    }

                    await using (new RaiiAsync<int>(Initialize, _ => Task.CompletedTask))
                    {
                        initialized.Should().Be(value);
                    }
                }

                [Fact]
                public async Task Should_Initialize_Context()
                {
                    var value = Create<int>();

                    int Initialize()
                    {
                        return value;
                    }

                    await using (var raii = new RaiiAsync<int>(Initialize, _ => Task.CompletedTask))
                    {
                        raii.Context.Should().Be(value);
                    }
                }

                [Fact]
                public async Task Should_Pass_Initialize_To_Cleanup()
                {
                    var value = Create<int>();
                    var initValue = value;

                    int Initialize()
                    {
                        return value;
                    }

                    await using (new RaiiAsync<int>(
                        Initialize,
                        val =>
                        {
                            initValue = val;
                            return Task.CompletedTask;
                        }))
                    {
                    }

                    initValue.Should().Be(value);
                }

                [Fact]
                public async Task Should_Not_Call_Cleanup()
                {
                    var cleanedUp = false;

                    Task Cleanup(int _)
                    {
                        cleanedUp = true;
                        return Task.CompletedTask;
                    }

                    await using (new RaiiAsync<int>(Create<int>, Cleanup))
                    {
                        cleanedUp.Should().BeFalse();
                    }
                }

                [Fact]
                public async Task Should_Call_Cleanup()
                {
                    var cleanedUp = false;

                    Task Cleanup(int _)
                    {
                        cleanedUp = true;
                        return Task.CompletedTask;
                    }

                    await using (new RaiiAsync<int>(Create<int>, Cleanup))
                    {
                    }

                    cleanedUp.Should().BeTrue();
                }
            }

            public class Disposal : Raii_NonType
            {
                [Fact]
                public async Task Should_Dispose()
                {
                    var cleanedUp = false;

                    Task Cleanup(int _)
                    {
                        cleanedUp = true;
                        return Task.CompletedTask;
                    }

                    await using (new RaiiAsync<int>(Create<int>, Cleanup))
                    {
                    }

                    cleanedUp.Should().BeTrue();
                }
            }
        }
    }
}