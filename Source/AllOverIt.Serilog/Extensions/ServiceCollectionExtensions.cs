using AllOverIt.Assertion;
using AllOverIt.Serilog.Sinks.CircularBuffer;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.Serilog.Extensions
{
    /// <summary>Provides extension methods for a <see cref="IServiceCollection"/>.</summary>
    public static class ServiceCollectionExtensions
    {
        private static readonly Type ICircularBufferSinkMessagesType = typeof(ICircularBufferSinkMessages);

        /// <summary>Adds a <see cref="ICircularBufferSinkMessages"/> sink to Serilog.</summary>
        /// <param name="services">The service collection to register the message sink.</param>
        /// <param name="capacity">The maximum number of messages to capture.</param>
        /// <param name="lifetime">The lifetime of the message sink.</param>
        /// <returns>The same <see cref="IServiceCollection"/> to allow for chained calls.</returns>
        public static IServiceCollection AddSerilogCircularBuffer(this IServiceCollection services, int capacity,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            _ = services.WhenNotNull(nameof(services));

            Throw<ArgumentOutOfRangeException>.When(capacity < 1, nameof(capacity), "The circular buffer requires a capacity of at least 1.");

            var descriptor = new ServiceDescriptor(
                ICircularBufferSinkMessagesType,
                _ => new CircularBufferSinkMessages(capacity),
                lifetime);

            services.Add(descriptor);

            return services;
        }

        /// <summary>Adds a <see cref="ICircularBufferSinkMessages"/> sink to Serilog.</summary>
        /// <param name="services">The service collection to register the message sink.</param>
        /// <param name="configure">The configuration action that returns the maximum number of messages to capture.</param>
        /// <param name="lifetime">The lifetime of the message sink.</param>
        /// <returns>The same <see cref="IServiceCollection"/> to allow for chained calls.</returns>
        public static IServiceCollection AddSerilogCircularBuffer(this IServiceCollection services, Func<IServiceProvider, int> configure,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            _ = services.WhenNotNull(nameof(services));
            _ = configure.WhenNotNull(nameof(configure));

            var descriptor = new ServiceDescriptor(
                ICircularBufferSinkMessagesType,
                provider =>
                {
                    var capacity = configure.Invoke(provider);

                    Throw<ArgumentOutOfRangeException>.When(capacity < 1, nameof(capacity), "The circular buffer requires a capacity of at least 1.");

                    return new CircularBufferSinkMessages(capacity);
                },
                lifetime);

            services.Add(descriptor);

            return services;
        }
    }
}
