using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pipes.Anonymous;
using FluentAssertions;
using System;
using System.IO;
using System.IO.Pipes;
using Xunit;

namespace AllOverIt.Pipes.Tests.Anonymous
{
    public class AnonymousPipeClientFixture : FixtureBase
    {
        [Collection("AnonymousPipes")]
        public class Start_Handle : AnonymousPipeClientFixture
        {
            [Fact]
            public void Should_Throw_When_Client_Handle_Null()
            {
                using (var client = new AnonymousPipeClient())
                {
                    Invoking(() =>
                    {
                        client.Start(null);
                    })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("clientHandle");
                }
            }

            [Fact]
            public void Should_Throw_When_Client_Handle_Empty()
            {
                using (var client = new AnonymousPipeClient())
                {
                    Invoking(() =>
                    {
                        client.Start(string.Empty);
                    })
                   .Should()
                   .Throw<ArgumentException>()
                   .WithNamedMessageWhenEmpty("clientHandle");
                }
            }

            [Fact]
            public void Should_Throw_When_Client_Handle_Whitespace()
            {
                using (var client = new AnonymousPipeClient())
                {
                    Invoking(() =>
                    {
                        client.Start("  ");
                    })
                   .Should()
                   .Throw<ArgumentException>()
                   .WithNamedMessageWhenEmpty("clientHandle");
                }
            }

            [Fact]
            public void Should_Throw_When_Initialized_Twice()
            {
                using (var server = new AnonymousPipeServer())
                {
                    var clientHandle = server.Start(PipeDirection.In, Create<HandleInheritability>());

                    using (var client = new AnonymousPipeClient())
                    {
                        client.Start(clientHandle);

                        Invoking(() =>
                        {
                            client.Start(PipeDirection.In, clientHandle);
                        })
                       .Should()
                       .Throw<InvalidOperationException>()
                       .WithMessage("The anonymous pipe has already been initialized.");
                    }
                }
            }
        }

        [Collection("AnonymousPipes")]
        public class Start_Direction_Handle : AnonymousPipeClientFixture
        {
            [Fact]
            public void Should_Throw_When_Client_Handle_Null()
            {
                using (var client = new AnonymousPipeClient())
                {
                    Invoking(() =>
                    {
                        client.Start(Create<PipeDirection>(), null);
                    })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("clientHandle");
                }
            }

            [Fact]
            public void Should_Throw_When_Client_Handle_Empty()
            {
                using (var client = new AnonymousPipeClient())
                {
                    Invoking(() =>
                    {
                        client.Start(Create<PipeDirection>(), string.Empty);
                    })
                   .Should()
                   .Throw<ArgumentException>()
                   .WithNamedMessageWhenEmpty("clientHandle");
                }
            }

            [Fact]
            public void Should_Throw_When_Client_Handle_Whitespace()
            {
                using (var client = new AnonymousPipeClient())
                {
                    Invoking(() =>
                    {
                        client.Start(Create<PipeDirection>(), "  ");
                    })
                   .Should()
                   .Throw<ArgumentException>()
                   .WithNamedMessageWhenEmpty("clientHandle");
                }
            }

            [Fact]
            public void Should_Throw_When_Initialized_Twice()
            {
                using (var server = new AnonymousPipeServer())
                {
                    var clientHandle = server.Start(PipeDirection.In, Create<HandleInheritability>());

                    using (var client = new AnonymousPipeClient())
                    {
                        client.Start(PipeDirection.Out, clientHandle);

                        Invoking(() =>
                        {
                            client.Start(PipeDirection.Out, clientHandle);
                        })
                       .Should()
                       .Throw<InvalidOperationException>()
                       .WithMessage("The anonymous pipe has already been initialized.");
                    }
                }
            }
        }

        [Collection("AnonymousPipes")]
        public class Reader : AnonymousPipeClientFixture
        {
            [Fact]
            public void Should_Throw_When_Cannot_Read()
            {
                using (var server = new AnonymousPipeServer())
                {
                    var clientHandle = server.Start(PipeDirection.In, Create<HandleInheritability>());

                    using (var client = new AnonymousPipeClient())
                    {
                        client.Start(PipeDirection.Out, clientHandle);

                        Invoking(() =>
                        {
                            _ = client.Reader;
                        })
                       .Should()
                       .Throw<InvalidOperationException>()
                       .WithMessage("The anonymous pipe is write-only.");
                    }
                }
            }

            [Fact]
            public void Should_Throw_When_Not_Initialized()
            {
                using (var server = new AnonymousPipeServer())
                {
                    var clientHandle = server.Start(PipeDirection.Out, Create<HandleInheritability>());

                    using (var client = new AnonymousPipeClient())
                    {
                        // Not calling client.Start(PipeDirection.In, clientHandle);

                        Invoking(() =>
                        {
                            _ = client.Reader;
                        })
                       .Should()
                       .Throw<InvalidOperationException>()
                       .WithMessage("The anonymous pipe has not been initialized. Call the InitializeStart() method.");
                    }
                }
            }
        }

        [Collection("AnonymousPipes")]
        public class Writer : AnonymousPipeClientFixture
        {
            [Fact]
            public void Should_Throw_When_Cannot_Write()
            {
                using (var server = new AnonymousPipeServer())
                {
                    var clientHandle = server.Start(PipeDirection.Out, Create<HandleInheritability>());

                    using (var client = new AnonymousPipeClient())
                    {
                        client.Start(PipeDirection.In, clientHandle);

                        Invoking(() =>
                        {
                            _ = client.Writer;
                        })
                       .Should()
                       .Throw<InvalidOperationException>()
                       .WithMessage("The anonymous pipe is read-only.");
                    }
                }
            }

            [Fact]
            public void Should_Throw_When_Not_Initialized()
            {
                using (var server = new AnonymousPipeServer())
                {
                    var clientHandle = server.Start(PipeDirection.In, Create<HandleInheritability>());

                    using (var client = new AnonymousPipeClient())
                    {
                        // Not calling client.Start(PipeDirection.Out, clientHandle);

                        Invoking(() =>
                        {
                            _ = client.Writer;
                        })
                       .Should()
                       .Throw<InvalidOperationException>()
                       .WithMessage("The anonymous pipe has not been initialized. Call the InitializeStart() method.");
                    }
                }
            }
        }

        [Collection("AnonymousPipes")]
        public class WaitForPipeDrain : AnonymousPipeServerFixture
        {
            [Fact]
            public void Should_Throw_When_ReadOnly()
            {
                using (var server = new AnonymousPipeServer())
                {
                    var clientHandle = server.Start(PipeDirection.Out, Create<HandleInheritability>());

                    using (var client = new AnonymousPipeClient())
                    {
                        client.Start(PipeDirection.In, clientHandle);

                        Invoking(() =>
                        {
                            client.WaitForPipeDrain();
                        })
                       .Should()
                       .Throw<InvalidOperationException>()
                       .WithMessage("The anonymous pipe is read-only.");
                    }
                }
            }

            [Fact]
            public void Should_Throw_When_Not_Initialized()
            {
                using (var server = new AnonymousPipeServer())
                {
                    var clientHandle = server.Start(PipeDirection.Out, Create<HandleInheritability>());

                    using (var client = new AnonymousPipeClient())
                    {
                        // Not calling client.Start(PipeDirection.In, clientHandle);

                        Invoking(() =>
                        {
                            client.WaitForPipeDrain();
                        })
                       .Should()
                       .Throw<InvalidOperationException>()
                       .WithMessage("The anonymous pipe has not been initialized. Call the InitializeStart() method.");
                    }
                }
            }
        }
    }
}