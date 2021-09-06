using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Helpers;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Helpers
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
                            var _ = new RaiiAsync(null, () => Task.CompletedTask);
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
                            var _ = new RaiiAsync(() => { }, null);
                        })
                        .Should()
                        .Throw<ArgumentNullException>()
                        .WithNamedMessageWhenNull("cleanup");
                }

                [Fact]
                public async Task Should_Call_Initialize()
                {
                    var initialized = false;

                    Action initialize = () =>
                    {
                        initialized = true;
                    };

                    await using (new RaiiAsync(initialize, () => Task.CompletedTask))
                    {
                        initialized.Should().BeTrue();
                    }
                }

                [Fact]
                public async Task Should_Not_Call_Cleanup()
                {
                    var cleanedUp = false;

                    Func<Task> cleanup = () =>
                    {
                        cleanedUp = true;
                        return Task.CompletedTask;
                    };

                    await using(new RaiiAsync(() => { }, cleanup))
                    {
                        cleanedUp.Should().BeFalse();
                    }
                }

                [Fact]
                public async Task Should_Call_Cleanup()
                {
                    var cleanedUp = false;

                    Func<Task> cleanup = () =>
                    {
                        cleanedUp = true;
                        return Task.CompletedTask;
                    };

                    await using (new RaiiAsync(() => { }, cleanup))
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

                    Func<Task> cleanup = () =>
                    {
                        cleanedUp = true;
                        return Task.CompletedTask;
                    };

                    await using (new RaiiAsync(() => { }, cleanup))
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
                            var _ = new RaiiAsync<int>(null, _ => Task.CompletedTask);
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
                            var _ = new RaiiAsync<int>(() => value, null);
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

                    Func<int> initialize = () =>
                    {
                        initialized = value;
                        return value;
                    };

                    await using (new RaiiAsync<int>(initialize, _ => Task.CompletedTask))
                    {
                        initialized.Should().Be(value);
                    }
                }

                [Fact]
                public async Task Should_Initialize_Context()
                {
                    var value = Create<int>();

                    Func<int> initialize = () =>
                    {
                        return value;
                    };

                    await using (var raii = new RaiiAsync<int>(initialize, _ => Task.CompletedTask))
                    {
                        raii.Context.Should().Be(value);
                    }
                }

                [Fact]
                public async Task Should_Pass_Initialize_To_Cleanup()
                {
                    var value = Create<int>();
                    var initValue = value;

                    Func<int> initialize = () =>
                    {
                        return value;
                    };

                    await using (new RaiiAsync<int>(
                        initialize,
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

                    Func<int, Task> cleanup = _ =>
                    {
                        cleanedUp = true;
                        return Task.CompletedTask;
                    };

                    await using (new RaiiAsync<int>(Create<int>, cleanup))
                    {
                        cleanedUp.Should().BeFalse();
                    }
                }

                [Fact]
                public async Task Should_Call_Cleanup()
                {
                    var cleanedUp = false;

                    Func<int, Task> cleanup = _ =>
                    {
                        cleanedUp = true;
                        return Task.CompletedTask;
                    };

                    await using (new RaiiAsync<int>(Create<int>, cleanup))
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

                    Func<int, Task> cleanup = _ =>
                    {
                        cleanedUp = true;
                        return Task.CompletedTask;
                    };

                    await using (new RaiiAsync<int>(Create<int>, cleanup))
                    {
                    }

                    cleanedUp.Should().BeTrue();
                }
            }
        }
    }
}