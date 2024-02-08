using AllOverIt.Assertion;
using AllOverIt.Serilog.Sinks.CircularBuffer;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.Serilog.Extensions
{
    /// <summary>Provides extension methods for a <see cref="IServiceCollection"/>.</summary>
    public static class ServiceCollectionExtensions
    {
        private static readonly Type ICircularBufferSinkMessagesType = typeof(ICircularBufferSinkMessages);

        public static IServiceCollection AddSerilogCircularBuffer(this IServiceCollection services, int capacity,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            _ = services.WhenNotNull(nameof(services));

            var descriptor = new ServiceDescriptor(
                ICircularBufferSinkMessagesType,
                _ => new CircularBufferSinkMessages(capacity),
                lifetime);

            services.Add(descriptor);

            return services;
        }
    }
}
