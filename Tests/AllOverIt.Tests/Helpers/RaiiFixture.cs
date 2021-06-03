﻿using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Helpers;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Helpers
{
    public class RaiiFixture : AoiFixtureBase
    {
        public class Raii_NonType : RaiiFixture
        {
            public class Constructor : Raii_NonType
            {
                [Fact]
                public void Should_Throw_When_Initialize_Null()
                {
                    Invoking(() =>
                        {
                            var _ = new Raii(null, () => { });
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
                            var _ = new Raii(() => { }, null);
                        })
                        .Should()
                        .Throw<ArgumentNullException>()
                        .WithNamedMessageWhenNull("cleanup");
                }

                [Fact]
                public void Should_Call_Initialize()
                {
                    var initialized = false;

                    Action initialize = () =>
                    {
                        initialized = true;
                    };

                    using (new Raii(initialize, () => { }))
                    {
                        initialized.Should().BeTrue();
                    }
                }

                [Fact]
                public void Should_Not_Call_Cleanup()
                {
                    var cleanedUp = false;

                    Action cleanup = () =>
                    {
                        cleanedUp = true;
                    };

                    using (new Raii(() => { }, cleanup))
                    {
                        cleanedUp.Should().BeFalse();
                    }
                }

                [Fact]
                public void Should_Call_Cleanup()
                {
                    var cleanedUp = false;

                    Action cleanup = () =>
                    {
                        cleanedUp = true;
                    };

                    using (new Raii(() => { }, cleanup))
                    { }

                    cleanedUp.Should().BeTrue();
                }
            }

            public class Disposal : Raii_NonType
            {
                [Fact]
                public void Should_Dispose()
                {
                    var cleanedUp = false;

                    Action cleanup = () =>
                    {
                        cleanedUp = true;
                    };

                    using (new Raii(() => { }, cleanup))
                    { }

                    cleanedUp.Should().BeTrue();
                }
            }
        }

        public class Raii_Type : RaiiFixture
        {
            public class Constructor : Raii_NonType
            {
                [Fact]
                public void Should_Throw_When_Initialize_Null()
                {
                    Invoking(() =>
                        {
                            var _ = new Raii<int>(null, _ => { });
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
                            var _ = new Raii<int>(() => value, null);
                        })
                        .Should()
                        .Throw<ArgumentNullException>()
                        .WithNamedMessageWhenNull("cleanup");
                }

                [Fact]
                public void Should_Call_Initialize()
                {
                    var value = Create<int>();

                    var initialized = false;

                    Func<int> initialize = () =>
                    {
                        initialized = true;
                        return value;
                    };

                    using (new Raii<int>(initialize, _ => { }))
                    {
                        initialized.Should().BeTrue();
                    }
                }

                [Fact]
                public void Should_Pass_Initialize_To_Cleanup()
                {
                    var value = Create<int>();

                    var initValue = -1;

                    Func<int> initialize = () =>
                    {
                        return value;
                    };

                    using (new Raii<int>(initialize, val => { initValue = val; }))
                    { }

                    initValue.Should().Be(value);
                }

                [Fact]
                public void Should_Not_Call_Cleanup()
                {
                    var value = Create<int>();

                    var cleanedUp = false;

                    Action<int> cleanup = _ =>
                    {
                        cleanedUp = true;
                    };

                    using (new Raii<int>(() => value, cleanup))
                    {
                        cleanedUp.Should().BeFalse();
                    }
                }

                [Fact]
                public void Should_Call_Cleanup()
                {
                    var value = Create<int>();

                    var cleanedUp = false;

                    Action<int> cleanup = _ =>
                    {
                        cleanedUp = true;
                    };

                    using (new Raii<int>(() => value, cleanup))
                    { }

                    cleanedUp.Should().BeTrue();
                }
            }

            public class Disposal : Raii_NonType
            {
                [Fact]
                public void Should_Dispose()
                {
                    var cleanedUp = false;

                    Action<int> cleanup = _ =>
                    {
                        cleanedUp = true;
                    };

                    using (new Raii<int>(Create<int>, cleanup))
                    { }

                    cleanedUp.Should().BeTrue();
                }
            }
        }
    }
}