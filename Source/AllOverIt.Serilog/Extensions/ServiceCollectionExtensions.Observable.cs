using AllOverIt.Assertion;
using AllOverIt.Serilog.Sinks.Observable;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.Serilog.Extensions
{
    /// <summary>Provides extension methods for a <see cref="IServiceCollection"/>.</summary>
    public static partial class ServiceCollectionExtensions
    {
        private static readonly Type IObservableSinkType = typeof(IObservableSink);

        /// <summary>Adds an <see cref="IObservableSink"/> sink to Serilog.</summary>
        /// <param name="services">The service collection to register the observable sink.</param>
        /// <param name="lifetime">The lifetime of the observable sink.</param>
        /// <returns>The same <see cref="IServiceCollection"/> to allow for chained calls.</returns>
        public static IServiceCollection AddSerilogObservable(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            _ = services.WhenNotNull(nameof(services));

            var descriptor = new ServiceDescriptor(
                IObservableSinkType,
                _ => new ObservableSink(),
                lifetime);

            services.Add(descriptor);

            return services;
        }
    }
}
