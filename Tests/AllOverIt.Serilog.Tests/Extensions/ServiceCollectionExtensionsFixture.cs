using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Serilog.Extensions;
using AllOverIt.Serilog.Sinks.CircularBuffer;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace AllOverIt.Serilog.Tests.Extensions
{
    public class ServiceCollectionExtensionsFixture : FixtureBase
    {
        private readonly IServiceCollection _services = new ServiceCollection();

        public class AddSerilogCircularBuffer : ServiceCollectionExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_ServiceCollection_Null()
            {
                Invoking(() =>
                {
                    _ = ServiceCollectionExtensions.AddSerilogCircularBuffer(null, Create<int>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("services");
            }

            [Fact]
            public void Should_Throw_When_Invalid_Capacity()
            {
                Invoking(() =>
                {
                    _ = ServiceCollectionExtensions.AddSerilogCircularBuffer(_services, 0);
                })
                .Should()
                .Throw<ArgumentOutOfRangeException>()
                .WithMessage("The circular buffer requires a capacity of at least 1. (Parameter 'capacity')");
            }

            [Fact]
            public void Should_Default_Register_Buffer_As_Scoped()
            {
                _ = ServiceCollectionExtensions.AddSerilogCircularBuffer(_services, Create<int>());

                _services
                    .Where(descriptor => descriptor.ServiceType == typeof(ICircularBufferSinkMessages))
                    .SingleOrDefault()
                    .Lifetime
                    .Should()
                    .Be(ServiceLifetime.Scoped);
            }

            [Fact]
            public void Should_Resolve_Same_Sink()
            {
                var capacity = Create<int>();

                _ = ServiceCollectionExtensions.AddSerilogCircularBuffer(_services, capacity);

                var provider = _services.BuildServiceProvider();

                var actual1 = provider.GetRequiredService<ICircularBufferSinkMessages>();

                actual1.Should().BeOfType<CircularBufferSinkMessages>();
                actual1.Capacity.Should().Be(capacity);

                var actual2 = provider.GetRequiredService<ICircularBufferSinkMessages>();

                actual1.Should().BeSameAs(actual2);
            }

            [Fact]
            public void Should_Resolve_Different_Sink()
            {
                var capacity = Create<int>();

                _ = ServiceCollectionExtensions.AddSerilogCircularBuffer(_services, capacity, ServiceLifetime.Transient);

                var provider = _services.BuildServiceProvider();

                var actual1 = provider.GetRequiredService<ICircularBufferSinkMessages>();

                actual1.Should().BeOfType<CircularBufferSinkMessages>();
                actual1.Capacity.Should().Be(capacity);

                var actual2 = provider.GetRequiredService<ICircularBufferSinkMessages>();

                actual1.Should().NotBeSameAs(actual2);
            }

            [Fact]
            public void Should_Return_ServiceCollection()
            {
                var actual = ServiceCollectionExtensions.AddSerilogCircularBuffer(_services, Create<int>());

                actual.Should().BeSameAs(_services);
            }
        }
    }
}