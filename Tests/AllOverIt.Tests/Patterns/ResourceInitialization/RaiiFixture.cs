using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.ResourceInitialization;

namespace AllOverIt.Tests.Patterns.ResourceInitialization
{
    public class RaiiFixture : FixtureBase
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
                            _ = new Raii(null, () => { });
                        })
                        .ShouldThrow<ArgumentNullException>()
                        .WithNamedMessageWhenNull("initialize");
                }

                [Fact]
                public void Should_Throw_When_Cleanup_Null()
                {
                    Invoking(() =>
                        {
                            _ = new Raii(() => { }, null);
                        })
                        .ShouldThrow<ArgumentNullException>()
                        .WithNamedMessageWhenNull("cleanup");
                }

                [Fact]
                public void Should_Call_Initialize()
                {
                    var initialized = false;

                    void Initialize()
                    {
                        initialized = true;
                    }

                    using (new Raii(Initialize, () => { }))
                    {
                        initialized.ShouldBeTrue();
                    }
                }

                [Fact]
                public void Should_Not_Call_Cleanup()
                {
                    var cleanedUp = false;

                    void Cleanup()
                    {
                        cleanedUp = true;
                    }

                    using (new Raii(() => { }, Cleanup))
                    {
                        cleanedUp.ShouldBeFalse();
                    }
                }

                [Fact]
                public void Should_Call_Cleanup()
                {
                    var cleanedUp = false;

                    void Cleanup()
                    {
                        cleanedUp = true;
                    }

                    using (new Raii(() => { }, Cleanup))
                    { }

                    cleanedUp.ShouldBeTrue();
                }
            }

            public class Disposal : Raii_NonType
            {
                [Fact]
                public void Should_Dispose()
                {
                    var cleanedUp = false;

                    void Cleanup()
                    {
                        cleanedUp = true;
                    }

                    using (new Raii(() => { }, Cleanup))
                    {
                    }

                    cleanedUp.ShouldBeTrue();
                }
            }
        }

        public class Raii_Type : RaiiFixture
        {
            public class Constructor : Raii_Type
            {
                [Fact]
                public void Should_Throw_When_Initialize_Null()
                {
                    Invoking(() =>
                        {
                            _ = new Raii<int>(null, _ => { });
                        })
                        .ShouldThrow<ArgumentNullException>()
                        .WithNamedMessageWhenNull("initialize");
                }

                [Fact]
                public void Should_Throw_When_Cleanup_Null()
                {
                    var value = Create<int>();

                    Invoking(() =>
                        {
                            _ = new Raii<int>(() => value, null);
                        })
                        .ShouldThrow<ArgumentNullException>()
                        .WithNamedMessageWhenNull("cleanup");
                }

                [Fact]
                public void Should_Call_Initialize()
                {
                    var value = Create<int>();
                    var initialized = -value;

                    int Initialize()
                    {
                        initialized = value;
                        return value;
                    }

                    using (new Raii<int>(Initialize, _ => { }))
                    {
                        initialized.ShouldBe(value);
                    }
                }

                [Fact]
                public void Should_Initialize_Context()
                {
                    var value = Create<int>();

                    int Initialize()
                    {
                        return value;
                    }

                    using (var raii = new Raii<int>(Initialize, _ => { }))
                    {
                        raii.Context.ShouldBe(value);
                    }
                }

                [Fact]
                public void Should_Pass_Initialize_To_Cleanup()
                {
                    var value = Create<int>();
                    var initValue = value;

                    int Initialize()
                    {
                        return value;
                    }

                    using (new Raii<int>(Initialize, val => { initValue = val; }))
                    {
                    }

                    initValue.ShouldBe(value);
                }

                [Fact]
                public void Should_Not_Call_Cleanup()
                {
                    var cleanedUp = false;

                    void Cleanup(int _)
                    {
                        cleanedUp = true;
                    }

                    using (new Raii<int>(Create<int>, Cleanup))
                    {
                        cleanedUp.ShouldBeFalse();
                    }
                }

                [Fact]
                public void Should_Call_Cleanup()
                {
                    var cleanedUp = false;

                    void Cleanup(int _)
                    {
                        cleanedUp = true;
                    }

                    using (new Raii<int>(Create<int>, Cleanup))
                    {
                    }

                    cleanedUp.ShouldBeTrue();
                }
            }

            public class Disposal : Raii_NonType
            {
                [Fact]
                public void Should_Dispose()
                {
                    var cleanedUp = false;

                    void Cleanup(int _)
                    {
                        cleanedUp = true;
                    }

                    using (new Raii<int>(Create<int>, Cleanup))
                    {
                    }

                    cleanedUp.ShouldBeTrue();
                }
            }
        }
    }
}



