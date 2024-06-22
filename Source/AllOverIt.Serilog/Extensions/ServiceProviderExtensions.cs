using AllOverIt.Assertion;
using AllOverIt.Serilog.Sinks.CircularBuffer;
using AllOverIt.Serilog.Sinks.Observable;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.Serilog.Extensions
{
    /// <summary>Provides extension methods for a <see cref="IServiceProvider"/>.</summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>Gets the <see cref="ICircularBufferSinkMessages"/> registered with the service collection.</summary>
        /// <param name="serviceProvider">The service provider to retrieve the <see cref="ICircularBufferSinkMessages"/> instance.</param>
        /// <returns>The <see cref="ICircularBufferSinkMessages"/> instance.</returns>
        public static ICircularBufferSinkMessages GetCircularBufferSinkMessages(this IServiceProvider serviceProvider)
        {
            _ = serviceProvider.WhenNotNull(nameof(serviceProvider));

            return serviceProvider.GetRequiredService<ICircularBufferSinkMessages>();
        }

        /// <summary>Gets the <see cref="IObservableSink"/> registered with the service collection.</summary>
        /// <param name="serviceProvider">The service provider to retrieve the <see cref="IObservableSink"/> instance.</param>
        /// <returns>The <see cref="IObservableSink"/> instance.</returns>
        public static IObservableSink GetObservableSink(this IServiceProvider serviceProvider)
        {
            _ = serviceProvider.WhenNotNull(nameof(serviceProvider));

            return serviceProvider.GetRequiredService<IObservableSink>();
        }
    }
}
