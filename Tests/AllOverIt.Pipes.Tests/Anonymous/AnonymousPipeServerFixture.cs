using AllOverIt.Fixture;
using AllOverIt.Pipes.Anonymous;
using FluentAssertions;
using System;
using System.IO;
using System.IO.Pipes;
using Xunit;

namespace AllOverIt.Pipes.Tests.Anonymous
{
    public class AnonymousPipeServerFixture : FixtureBase
    {
        [Collection("Pipes")]
        public class Start : AnonymousPipeServerFixture
        {
            [Fact]
            public void Should_Return_Client_Handle()
            {
                using (var server = new AnonymousPipeServer())
                {
                    var actual = server.Start(CreateExcluding(PipeDirection.InOut), Create<HandleInheritability>());

                    actual.Should().NotBeNullOrEmpty();
                }
            }

            [Fact]
            public void Should_Throw_When_Initialized_Twice()
            {
                using (var server = new AnonymousPipeServer())
                {
                    _ = server.Start(PipeDirection.In, Create<HandleInheritability>());

                    Invoking(() =>
                    {
                        server.Start(CreateExcluding(PipeDirection.InOut), Create<HandleInheritability>());
                    })
                   .Should()
                   .Throw<InvalidOperationException>()
                   .WithMessage("The anonymous pipe has already been initialized.");
                }
            }
        }

        [Collection("Pipes")]
        public class Reader : AnonymousPipeServerFixture
        {
            [Fact]
            public void Should_Throw_When_Cannot_Read()
            {
                using (var server = new AnonymousPipeServer())
                {
                    _ = server.Start(PipeDirection.Out, Create<HandleInheritability>());

                    Invoking(() =>
                    {
                        _ = server.Reader;
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage("The anonymous pipe is write-only.");
                }
            }

            [Fact]
            public void Should_Throw_When_Not_Initialized()
            {
                using (var server = new AnonymousPipeServer())
                {
                    // Not calling server.Start(PipeDirection.In, Create<HandleInheritability>());

                    Invoking(() =>
                    {
                        _ = server.Reader;
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                       .WithMessage("The anonymous pipe has not been initialized. Call the InitializeStart() method.");
                }
            }
        }

        [Collection("Pipes")]
        public class Writer : AnonymousPipeServerFixture
        {
            [Fact]
            public void Should_Throw_When_Cannot_Write()
            {
                using (var server = new AnonymousPipeServer())
                {
                    _ = server.Start(PipeDirection.In, Create<HandleInheritability>());

                    Invoking(() =>
                    {
                        _ = server.Writer;
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage("The anonymous pipe is read-only.");
                }
            }

            [Fact]
            public void Should_Throw_When_Not_Initialized()
            {
                using (var server = new AnonymousPipeServer())
                {
                    // Not calling server.Start(PipeDirection.Out, Create<HandleInheritability>());

                    Invoking(() =>
                    {
                        _ = server.Writer;
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                       .WithMessage("The anonymous pipe has not been initialized. Call the InitializeStart() method.");
                }
            }
        }

        [Collection("Pipes")]
        public class WaitForPipeDrain : AnonymousPipeServerFixture
        {
            [Fact]
            public void Should_Throw_When_ReadOnly()
            {
                using (var server = new AnonymousPipeServer())
                {
                    _ = server.Start(PipeDirection.In, Create<HandleInheritability>());

                    Invoking(() =>
                    {
                        server.WaitForPipeDrain();
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage("The anonymous pipe is read-only.");
                }
            }

            [Fact]
            public void Should_Throw_When_Not_Initialized()
            {
                using (var server = new AnonymousPipeServer())
                {
                    // Not calling server.Start(PipeDirection.Out, Create<HandleInheritability>());

                    Invoking(() =>
                    {
                        server.WaitForPipeDrain();
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage("The anonymous pipe has not been initialized. Call the InitializeStart() method.");
                }
            }
        }
    }
}