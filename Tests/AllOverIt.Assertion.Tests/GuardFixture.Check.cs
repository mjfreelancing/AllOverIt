using AllOverIt.Assertion;
using AllOverIt.Fixture.Extensions;
using Shouldly;

namespace AllOverIt.Tests.Assertion
{
    public partial class GuardFixture
    {
        public class CheckNotNull_Type : GuardFixture
        {
            [Fact]
            public void Should_Throw_With_Expected_Name()
            {
                DummyClass dummy = null;

                Should.Throw<InvalidOperationException>(() =>
                    {
                        Guard.CheckNotNull(dummy);
                    })
                    .WithNamedMessageWhenNull(nameof(dummy));
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                var name = Create<string>();

                Should.Throw<InvalidOperationException>(() =>
                    {
                        DummyClass dummy = null;

                        Guard.CheckNotNull(dummy, name);
                    })
                    .WithNamedMessageWhenNull(name);
            }

            [Fact]
            public void Should_Throw_Message_When_Null()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();

                Should.Throw<InvalidOperationException>(() =>
                    {
                        DummyClass dummy = null;

                        Guard.CheckNotNull(dummy, name, errorMessage);
                    })
                    .WithNamedMessageWhenNull(name, errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Should.NotThrow(() =>
                    {
                        var dummy = new DummyClass();

                        Guard.CheckNotNull(dummy, Create<string>());
                    });
            }
        }

        public class CheckIsNull_Type : GuardFixture
        {
            [Fact]
            public void Should_Throw_With_Expected_Name()
            {
                var dummy = Create<DummyClass>();

                Should.Throw<InvalidOperationException>(() =>
                    {
                        Guard.CheckIsNull(dummy);
                    })
                    .WithNamedMessageWhenNotNull(nameof(dummy));
            }

            [Fact]
            public void Should_Throw_When_Not_Null()
            {
                var name = Create<string>();

                Should.Throw<InvalidOperationException>(() =>
                    {
                        var dummy = Create<DummyClass>();

                        Guard.CheckIsNull(dummy, name);
                    })
                    .WithNamedMessageWhenNotNull(name);
            }

            [Fact]
            public void Should_Throw_Message_When_Not_Null()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();

                Should.Throw<InvalidOperationException>(() =>
                    {
                        var dummy = Create<DummyClass>();

                        Guard.CheckIsNull(dummy, name, errorMessage);
                    })
                    .WithNamedMessageWhenNotNull(name, errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Should.NotThrow(() =>
                    {
                        Guard.CheckIsNull((DummyClass)null, Create<string>());
                    });
            }
        }

        public class CheckNotNullOrEmpty_Type : GuardFixture
        {
            [Fact]
            public void Should_Throw_With_Expected_Name()
            {
                IEnumerable<DummyClass> dummy = null;

                Should.Throw<InvalidOperationException>(() =>
                    {
                        Guard.CheckNotNullOrEmpty(dummy);
                    })
                    .WithNamedMessageWhenNull(nameof(dummy));
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                var name = Create<string>();

                Should.Throw<InvalidOperationException>(() =>
                    {
                        IEnumerable<DummyClass> dummy = null;

                        Guard.CheckNotNullOrEmpty(dummy, name);
                    })
                    .WithNamedMessageWhenNull(name);
            }

            [Fact]
            public void Should_Throw_Message_When_Null()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();

                Should.Throw<InvalidOperationException>(() =>
                    {
                        IEnumerable<DummyClass> dummy = null;

                        Guard.CheckNotNullOrEmpty(dummy, name, errorMessage);
                    })
                    .WithNamedMessageWhenNull(name, errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var name = Create<string>();
                var expected = new List<DummyClass>();

                Should.Throw<InvalidOperationException>(() =>
                    {
                        Guard.CheckNotNullOrEmpty(expected, name);
                    })
                    .WithNamedMessageWhenEmpty(name);
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();
                var expected = new List<DummyClass>();

                Should.Throw<InvalidOperationException>(() =>
                    {
                        Guard.CheckNotNullOrEmpty(expected, name, errorMessage);
                    })
                    .WithNamedMessageWhenEmpty(name, errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Should.NotThrow(() =>
                    {
                        var dummy = new List<DummyClass> { new DummyClass() };

                        Guard.CheckNotNullOrEmpty(dummy, Create<string>());
                    });
            }

            [Fact]
            public void Should_Not_Throw_Message()
            {
                var errorMessage = Create<string>();

                Should.NotThrow(() =>
                    {
                        var dummy = new List<DummyClass> { new DummyClass() };

                        Guard.CheckNotNullOrEmpty(dummy, Create<string>(), errorMessage);
                    });
            }
        }

        public class CheckNotEmpty_Type : GuardFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Null()
            {
                var name = Create<string>();

                Should.NotThrow(() =>
                    {
                        IEnumerable<DummyClass> dummy = null;

                        Guard.CheckNotEmpty(dummy, name);
                    });
            }

            [Fact]
            public void Should_Throw_With_Expected_Name()
            {
                var expected = new List<DummyClass>();

                Should.Throw<InvalidOperationException>(() =>
                    {
                        Guard.CheckNotEmpty(expected);
                    })
                    .WithNamedMessageWhenEmpty(nameof(expected));
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var name = Create<string>();
                var expected = new List<DummyClass>();

                Should.Throw<InvalidOperationException>(() =>
                    {
                        Guard.CheckNotEmpty(expected, name);
                    })
                    .WithNamedMessageWhenEmpty(name);
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();
                var expected = new List<DummyClass>();

                Should.Throw<InvalidOperationException>(() =>
                    {
                        Guard.CheckNotEmpty(expected, name, errorMessage);
                    })
                    .WithNamedMessageWhenEmpty(name, errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Should.NotThrow(() =>
                    {
                        var dummy = new List<DummyClass> { new DummyClass() };

                        Guard.CheckNotEmpty(dummy, Create<string>());
                    });
            }

            [Fact]
            public void Should_Not_Throw_Message()
            {
                var errorMessage = Create<string>();

                Should.NotThrow(() =>
                    {
                        var dummy = new List<DummyClass> { new DummyClass() };

                        Guard.CheckNotEmpty(dummy, Create<string>(), errorMessage);
                    });
            }
        }

        public class CheckNotNullOrEmpty_String : GuardFixture
        {
            [Fact]
            public void Should_Throw_With_Expected_Name()
            {
                string dummy = null;

                Should.Throw<InvalidOperationException>(() =>
                    {
                        Guard.CheckNotNullOrEmpty(dummy);
                    })
                    .WithNamedMessageWhenNull(nameof(dummy));
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                var name = Create<string>();

                Should.Throw<InvalidOperationException>(() =>
                    {
                        string dummy = null;

                        Guard.CheckNotNullOrEmpty(dummy, name);
                    })
                    .WithNamedMessageWhenNull(name);
            }

            [Fact]
            public void Should_Throw_Message_When_Null()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();

                Should.Throw<InvalidOperationException>(() =>
                    {
                        string dummy = null;

                        Guard.CheckNotNullOrEmpty(dummy, name, errorMessage);
                    })
                    .WithNamedMessageWhenNull(name, errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var name = Create<string>();
                var expected = string.Empty;

                Should.Throw<InvalidOperationException>(() =>
                    {
                        Guard.CheckNotNullOrEmpty(expected, name);
                    })
                    .WithNamedMessageWhenEmpty(name);
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();
                var expected = string.Empty;

                Should.Throw<InvalidOperationException>(() =>
                    {
                        Guard.CheckNotNullOrEmpty(expected, name, errorMessage);
                    })
                    .WithNamedMessageWhenEmpty(name, errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Should.NotThrow(() =>
                    {
                        var dummy = Create<string>();

                        Guard.CheckNotNullOrEmpty(dummy, Create<string>());
                    });
            }

            [Fact]
            public void Should_Not_Throw_Message()
            {
                var errorMessage = Create<string>();

                Should.NotThrow(() =>
                    {
                        var dummy = Create<string>();

                        Guard.CheckNotNullOrEmpty(dummy, Create<string>(), errorMessage);
                    });
            }
        }

        public class CheckNotEmpty_String : GuardFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Null()
            {
                var name = Create<string>();

                Should.NotThrow(() =>
                    {
                        string dummy = null;

                        Guard.CheckNotEmpty(dummy, name);
                    });
            }

            [Fact]
            public void Should_Throw_With_Expected_Name()
            {
                var expected = string.Empty;

                Should.Throw<InvalidOperationException>(() =>
                    {
                        Guard.CheckNotEmpty(expected);
                    })
                    .WithNamedMessageWhenEmpty(nameof(expected));
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var name = Create<string>();
                var expected = string.Empty;

                Should.Throw<InvalidOperationException>(() =>
                    {
                        Guard.CheckNotEmpty(expected, name);
                    })
                    .WithNamedMessageWhenEmpty(name);
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();
                var expected = string.Empty;

                Should.Throw<InvalidOperationException>(() =>
                    {
                        Guard.CheckNotEmpty(expected, name, errorMessage);
                    })
                    .WithNamedMessageWhenEmpty(name, errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Should.NotThrow(() =>
                    {
                        var dummy = Create<string>();

                        Guard.CheckNotEmpty(dummy, Create<string>());
                    });
            }

            [Fact]
            public void Should_Not_Throw_Message()
            {
                var errorMessage = Create<string>();

                Should.NotThrow(() =>
                    {
                        var dummy = Create<string>();

                        Guard.CheckNotEmpty(dummy, Create<string>(), errorMessage);
                    });
            }
        }
    }
}
