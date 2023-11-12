using AllOverIt.Aspects.Interceptor;
using AllOverIt.DependencyInjection.Extensions;
using InterceptorDemo.Interceptors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace InterceptorDemo
{
    internal class Program
    {
        static async Task Main()
        {
            try
            {
                var services = new ServiceCollection();

                // 'SecretService' is the real service to be decorated / intercepted
                services.AddScoped<ISecretService, SecretService>();

                // Method 1: Use MethodInterceptor<T> to provide support for filtering the methods to be intercepted.
                services.DecorateWithInterceptor<ISecretService, MethodInterceptor<ISecretService>>((provider, interceptor) =>
                {
                    // Demonstrating how to return a result from BeforeInvoke() and hence not calling the decorated service
                    var useCache = true;

                    // Each handler can be configured via its' 'TargetMethods' property to indicate which method(s) it will handle.
                    interceptor
                        .AddMethodHandler(new GetSecretHandler(1000, useCache))
                        .AddMethodHandler(new GetSecretAsyncHandler(1000));
                });

                // Method 2: Register an interceptor without any filtering.
                //services.DecorateWithInterceptor<ISecretService, TimedInterceptor>((provider, interceptor) =>
                //{
                //    interceptor.MinimimReportableMilliseconds = 1000;
                //});

                var serviceProvider = services.BuildServiceProvider();

                var dispatchProxy = serviceProvider.GetRequiredService<ISecretService>();

                var accessKey = "some_key";

                var secret = dispatchProxy.GetSecret(accessKey);
                Console.WriteLine($"FINAL Result: {secret}");      // should be reported as 0-1ms

                // Adding this to make sure this time is not included in the time period reported by the proxy
                await Task.Delay(2000);

                Console.WriteLine();

                secret = await dispatchProxy.GetSecretAsync(accessKey, false);
                Console.WriteLine($"FINAL Result: {secret}");      // should be reported as approx. 1000ms

                Console.WriteLine();

                secret = await dispatchProxy.GetSecretAsync(accessKey, true);   // will throw
            }
            catch (Exception exception)
            {
                Console.WriteLine($"CAUGHT: {exception.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}