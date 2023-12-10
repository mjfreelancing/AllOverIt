#define METHOD_INTERCEPTOR

using AllOverIt.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

#if METHOD_INTERCEPTOR
using AllOverIt.Aspects;
using InterceptorDemo.Interceptors.MethodLevel;
#else
using InterceptorDemo.Interceptors.ClassLevel;
#endif

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

#if METHOD_INTERCEPTOR
                // Method 1: Use MethodInterceptor<T> to provide support for filtering the methods to be intercepted.
                services.DecorateWithInterceptor<ISecretService, MethodInterceptor<ISecretService>>((provider, interceptor) =>
                {
                    // Demonstrating how to return a result from BeforeInvoke() and hence not calling the decorated service
                    var useCache = true;

                    // Each handler can be configured via its' 'TargetMethods' property to indicate which method(s) it will handle.
                    interceptor
                        .AddMethodHandler(new GetSecretIdHandler())
                        .AddMethodHandler(new GetSecretHandler(1000, useCache))
                        .AddMethodHandler(new GetSecretAsyncHandler(1000));
                });
#else
                // Method 2: Register a class interceptor (any method filtering must be performed by the interceptor).
                services.DecorateWithInterceptor<ISecretService, TimedInterceptor>((provider, interceptor) =>
                {
                    interceptor.MinimimReportableMilliseconds = 1000;
                });
#endif

                var serviceProvider = services.BuildServiceProvider();

                var dispatchProxy = serviceProvider.GetRequiredService<ISecretService>();

                // The class-level interceptor filters out calls to Initialize(), InitializeAsync(), GetSecretId()
                dispatchProxy.Initialize();
                await dispatchProxy.InitializeAsync();
                var id = dispatchProxy.GetSecretId();
                Console.WriteLine($"Id: {id}");

                Console.WriteLine();

                var accessKey = "some_key";

                var secret = dispatchProxy.GetSecret(accessKey);
                Console.WriteLine($"FINAL Result: {secret}");      // should be reported as 0-1ms

                // Adding this to make sure this time is not included in the time period reported by the proxy
                await Task.Delay(750);

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