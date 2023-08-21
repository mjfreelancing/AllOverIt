using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pipes.Anonymous;
using FluentAssertions;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Pipes.Tests.Anonymous
{
    public class AnonymousPipeFixture_Functional : FixtureBase
    {
        [Fact]
        public async Task Server_Should_Write_To_Client()
        {
            var expected = Create<string>();
            string actual = default;

            using (var server = new AnonymousPipeServer())
            {
                var clientHandle = server.Start(PipeDirection.Out, HandleInheritability.Inheritable);

                using (var client = new AnonymousPipeClient())
                {
                    client.Start(PipeDirection.In, clientHandle);

                    var serverTask = Task.Run(() =>
                    {
                        server.Writer.WriteLine(expected);

                        server.WaitForPipeDrain();
                    });

                    await Task.Yield();

                    var clientTask = Task.Run(() =>
                    {
                        actual = client.Reader.ReadLine();
                    });

                    await Task.Yield();

                    await serverTask;
                    await clientTask;
                }
            }

            actual.Should().Be(expected);
        }

        [Fact]
        public async Task Server_Should_Read_From_Client()
        {
            var expected = Create<string>();
            string actual = default;

            using (var server = new AnonymousPipeServer())
            {
                var clientHandle = server.Start(PipeDirection.In, HandleInheritability.Inheritable);

                using (var client = new AnonymousPipeClient())
                {
                    client.Start(PipeDirection.Out, clientHandle);

                    var serverTask = Task.Run(() =>
                    {
                        actual = server.Reader.ReadLine();
                    });

                    var clientTask = Task.Run(() =>
                    {
                        client.Writer.WriteLine(expected);

                        client.WaitForPipeDrain();
                    });

                    await Task.WhenAll(clientTask, serverTask);
                }
            }

            actual.Should().Be(expected);
        }
    }
}