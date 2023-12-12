using AllOverIt.Aspects;
using AllOverIt.DependencyInjection.Extensions;
using InterceptorDemo.Interceptors.ClassLevel;
using InterceptorDemo.Interceptors.MethodLevel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace InterceptorDemo
{
    internal class Program
    {
        static async Task Main()
        {
            try
            {
                // Multiple different examples
                await TimeAllMethodExecutionsUsingClassInterceptor();
                await ChangeInputArgUsingClassInterceptor();
                await HandleResultUsingClassInterceptor();
                await ChangeFinalResultUsingClassInterceptor();
                await UseMethodInterceptors();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"CAUGHT: {exception.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static void LogCalledFrom([CallerMemberName] string callerName = "")
        {
            Console.WriteLine($"{callerName}:");
            Console.WriteLine();
        }

        private static Task TimeAllMethodExecutionsUsingClassInterceptor()
        {
            LogCalledFrom();

            var services = new ServiceCollection();

            // 'SecretService' is the real service to be decorated / intercepted
            services.AddScoped<ISecretService, SecretService>();

            services.DecorateWithInterceptor<ISecretService, TimeAllMethodExecutionsInterceptor>((provider, interceptor) =>
            {
                interceptor.MinimimReportableMilliseconds = 1;
            });

            var serviceProvider = services.BuildServiceProvider();

            // Will return a dispatch Proxy
            var dispatchProxy = serviceProvider.GetRequiredService<ISecretService>();

            return Run(dispatchProxy);
        }

        private static Task ChangeInputArgUsingClassInterceptor()
        {
            LogCalledFrom();

            var services = new ServiceCollection();

            // 'SecretService' is the real service to be decorated / intercepted
            services.AddScoped<ISecretService, SecretService>();

            services.DecorateWithInterceptor<ISecretService, ChangeInputArgInterceptor>();

            var serviceProvider = services.BuildServiceProvider();

            // Will return a dispatch Proxy
            var dispatchProxy = serviceProvider.GetRequiredService<ISecretService>();

            return Run(dispatchProxy);
        }

        private static Task HandleResultUsingClassInterceptor()
        {
            LogCalledFrom();

            var services = new ServiceCollection();

            // 'SecretService' is the real service to be decorated / intercepted
            services.AddScoped<ISecretService, SecretService>();

            services.DecorateWithInterceptor<ISecretService, HandleResultInterceptor>();

            var serviceProvider = services.BuildServiceProvider();

            // Will return a dispatch Proxy
            var dispatchProxy = serviceProvider.GetRequiredService<ISecretService>();

            return Run(dispatchProxy);
        }

        private static Task ChangeFinalResultUsingClassInterceptor()
        {
            LogCalledFrom();

            var services = new ServiceCollection();

            // 'SecretService' is the real service to be decorated / intercepted
            services.AddScoped<ISecretService, SecretService>();

            services.DecorateWithInterceptor<ISecretService, ChangeFinalResultInterceptor>();

            var serviceProvider = services.BuildServiceProvider();

            // Will return a dispatch Proxy
            var dispatchProxy = serviceProvider.GetRequiredService<ISecretService>();

            return Run(dispatchProxy);
        }

        private static Task UseMethodInterceptors()
        {
            LogCalledFrom();

            var services = new ServiceCollection();

            // 'SecretService' is the real service to be decorated / intercepted
            services.AddScoped<ISecretService, SecretService>();

            services.DecorateWithInterceptor<ISecretService, MethodInterceptor<ISecretService>>((provider, interceptor) =>
            {
                // Demonstrating how to return a result from BeforeInvoke() and hence not calling the decorated service instance
                var useCache = true;

                // Each handler can be configured via its 'TargetMethods' property to indicate which method(s) it will handle.
                interceptor
                    .AddMethodHandler(new GetSecretIdHandler())
                    .AddMethodHandler(new GetSecretHandler(1000, useCache))
                    .AddMethodHandler(new GetSecretAsyncHandler(1000));
            });

            var serviceProvider = services.BuildServiceProvider();

            // Will return a dispatch Proxy
            var dispatchProxy = serviceProvider.GetRequiredService<ISecretService>();

            return Run(dispatchProxy);
        }

        private static async Task Run(ISecretService dispatchProxy)
        {
            try
            {
                dispatchProxy.Initialize();
                Console.WriteLine();

                await dispatchProxy.InitializeAsync();
                Console.WriteLine();

                var result1 = dispatchProxy.GetSecretId();
                Console.WriteLine($"    => GetSecretId() Result = {result1}");
                Console.WriteLine();

                var result2 = dispatchProxy.GetSecret("secret access key");
                Console.WriteLine($"    => GetSecret() Result = {result2}");
                Console.WriteLine();

                // Adding this to make sure this time is not included in the time period reported by the proxy
                await Task.Delay(500);

                var result3 = await dispatchProxy.GetSecretAsync("async secret access key (no throw)", false);
                Console.WriteLine($"    => GetSecretAsync(no throw) Result = {result3}");
                Console.WriteLine();

                var result4 = await dispatchProxy.GetSecretAsync("async secret access key (throw)", true);
                Console.WriteLine($"    => GetSecretAsync(throw) Result = {result4}");
                Console.WriteLine();

            }
            catch (Exception exception)
            {
                Console.WriteLine($"CAUGHT: {exception.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine();
        }
    }
}