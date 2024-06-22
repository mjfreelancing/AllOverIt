using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Serilog.Extensions;
using AllOverIt.Serilog.Sinks.Observable;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.Serilog.Tests.Extensions
{
    public partial class ServiceCollectionExtensionsFixture : FixtureBase
    {
        public class AddSerilogObservable : ServiceCollectionExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_ServiceCollection_Null()
            {
                Invoking(() =>
                {
                    _ = ServiceCollectionExtensions.AddSerilogObservable(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("services");
            }

            [Fact]
            public void Should_Default_Register_Buffer_As_Scoped()
            {
                _ = ServiceCollectionExtensions.AddSerilogObservable(_services);

                _services
                    .Where(descriptor => descriptor.ServiceType == typeof(IObservableSink))
                    .SingleOrDefault()
                    .Lifetime
                    .Should()
                    .Be(ServiceLifetime.Scoped);
            }

            [Fact]
            public void Should_Resolve_Same_Sink()
            {
                var capacity = Create<int>();

                _ = ServiceCollectionExtensions.AddSerilogObservable(_services);

                var provider = _services.BuildServiceProvider();

                var actual1 = provider.GetRequiredService<IObservableSink>();

                actual1.Should().BeOfType<ObservableSink>();

                var actual2 = provider.GetRequiredService<IObservableSink>();

                actual1.Should().BeSameAs(actual2);
            }

            [Fact]
            public void Should_Resolve_Different_Sink()
            {
                var capacity = Create<int>();

                _ = ServiceCollectionExtensions.AddSerilogObservable(_services, ServiceLifetime.Transient);

                var provider = _services.BuildServiceProvider();

                var actual1 = provider.GetRequiredService<IObservableSink>();

                actual1.Should().BeOfType<ObservableSink>();

                var actual2 = provider.GetRequiredService<IObservableSink>();

                actual1.Should().NotBeSameAs(actual2);
            }

            [Fact]
            public void Should_Return_ServiceCollection()
            {
                var actual = ServiceCollectionExtensions.AddSerilogObservable(_services);

                actual.Should().BeSameAs(_services);
            }
        }
    }
}