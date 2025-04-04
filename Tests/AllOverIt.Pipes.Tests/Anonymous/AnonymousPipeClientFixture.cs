﻿using AllOverIt.Fixture;
using AllOverIt.Pipes.Anonymous;
using FluentAssertions;
using System.IO.Pipes;

namespace AllOverIt.Pipes.Tests.Anonymous
{
    public class AnonymousPipeClientFixture : FixtureBase
    {
        [Collection("AnonymousPipes")]
        public class Start_Handle : AnonymousPipeClientFixture
        {
            [Fact]
            public void Should_Throw_When_Client_Handle_Null_Empty_Whitespace()
            {
                using (var client = new AnonymousPipeClient())
                {
                    AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                        stringValue =>
                        {
                            client.Start(stringValue);
                        }, "clientHandle");
                }
            }

            [Fact]
            public async Task Should_Throw_When_Initialized_Twice()
            {
                using (var server = new AnonymousPipeServer())
                {
                    var clientHandle = server.Start(PipeDirection.In, Create<HandleInheritability>());

                    using (var client = new AnonymousPipeClient())
                    {
                        client.Start(clientHandle);

                        await Task.Delay(10);

                        Invoking(() =>
                        {
                            client.Start(PipeDirection.In, clientHandle);
                        })
                       .Should()
                       .Throw<InvalidOperationException>()
                       .WithMessage("The anonymous pipe client has already been started.");
                    }
                }
            }
        }

        [Collection("AnonymousPipes")]
        public class Start_Direction_Handle : AnonymousPipeClientFixture
        {
            [Fact]
            public void Should_Throw_When_Client_Handle_Null_Empty_Whitespace()
            {
                using (var client = new AnonymousPipeClient())
                {
                    AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                        stringValue =>
                        {
                            client.Start(Create<PipeDirection>(), stringValue);
                        }, "clientHandle");
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
                       .WithMessage("The anonymous pipe client has already been started.");
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