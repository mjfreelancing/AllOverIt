using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Serilog.Extensions;
using AllOverIt.Serilog.Sinks.CircularBuffer;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AllOverIt.Serilog.Tests.Extensions
{
    public class ServiceProviderExtensionsFixture : FixtureBase
    {
        [Fact]
        public void Should_Throw_When_ServiceProvider_Null()
        {
            Invoking(() =>
            {
                _ = ServiceProviderExtensions.GetCircularBufferSinkMessages(null);
            })
            .Should()
            .Throw<ArgumentNullException>()
            .WithNamedMessageWhenNull("serviceProvider");
        }

        [Fact]
        public void Should_Resolve_CircularBufferSinkMessages()
        {
            var services = new ServiceCollection();
            var bufferSize = Create<int>();

            services.AddSingleton<ICircularBufferSinkMessages>(new CircularBufferSinkMessages(bufferSize));

            var provider = services.BuildServiceProvider();

            var actual = ServiceProviderExtensions.GetCircularBufferSinkMessages(provider);

            actual.Should().BeOfType<CircularBufferSinkMessages>();

            actual.Capacity.Should().Be(bufferSize);
        }
    }
}