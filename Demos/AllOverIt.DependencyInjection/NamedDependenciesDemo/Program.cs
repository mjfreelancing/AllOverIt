using AllOverIt.DependencyInjection;
using AllOverIt.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace NamedDependenciesDemo
{
    internal class Program
    {
        static void Main()
        {
            var services = new ServiceCollection();

            // Register 3 implementations of IRepository - using generics
            services
                .AddNamedServices<IRepository>()
                .AsScoped<NumberRepository>("number1")
                .AsScoped<NumberRepository>("number2")      // Showing 2 registrations can point to the same type
                .AsScoped<GuidRepository>("guid")
                .AsScoped<DateTimeRepository>("datetime");

            // And 2 implementations of IClock - using typeof()
            services
                .AddNamedServices<IClock>()
                .AsScoped("clock", typeof(Clock))
                .AsScoped("utcclock", typeof(UtcClock));

            var provider = services.BuildServiceProvider();

            ResolveIRepositoryUsingTypedNameResolver(provider);
            ResolveIRepositoryUsingNonTypedNameResolver(provider);
            ResolveIClockUsingNonTypedNameResolver(provider);

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.WriteLine();
        }

        private static void ResolveIRepositoryUsingTypedNameResolver(IServiceProvider provider)
        {
            Console.WriteLine("Resolve IRepository via INamedDependencyResolver<IRepository>");
            Console.WriteLine("=============================================================");

            var repositoryResolver = provider.GetRequiredService<INamedServiceResolver<IRepository>>();

            var number1Repository = repositoryResolver.GetRequiredNamedService("number1");
            Console.WriteLine($"Number 1: {number1Repository.GetValue()}");

            var number2Repository = repositoryResolver.GetRequiredNamedService("number2");
            Console.WriteLine($"Number 2: {number2Repository.GetValue()}");

            var guidRepository = repositoryResolver.GetRequiredNamedService("guid");
            Console.WriteLine($"GUID: {guidRepository.GetValue()}");

            var dateTimeRepository = repositoryResolver.GetRequiredNamedService("datetime");
            Console.WriteLine($"DateTime: {dateTimeRepository.GetValue()}");

            Console.WriteLine();
        }

        private static void ResolveIRepositoryUsingNonTypedNameResolver(IServiceProvider provider)
        {
            Console.WriteLine("Resolve IRepository via INamedDependencyResolver");
            Console.WriteLine("================================================");

            var repositoryResolver = provider.GetRequiredService<INamedServiceResolver>();

            var number1Repository = repositoryResolver.GetRequiredNamedService<IRepository>("number1");
            Console.WriteLine($"Number 1: {number1Repository.GetValue()}");

            var number2Repository = repositoryResolver.GetRequiredNamedService<IRepository>("number2");
            Console.WriteLine($"Number 2: {number2Repository.GetValue()}");

            var guidRepository = repositoryResolver.GetRequiredNamedService<IRepository>("guid");
            Console.WriteLine($"GUID: {guidRepository.GetValue()}");

            var dateTimeRepository = repositoryResolver.GetRequiredNamedService<IRepository>("datetime");
            Console.WriteLine($"DateTime: {dateTimeRepository.GetValue()}");

            Console.WriteLine();
        }

        private static void ResolveIClockUsingNonTypedNameResolver(IServiceProvider provider)
        {
            Console.WriteLine("Resolve IClock via INamedDependencyResolver");
            Console.WriteLine("================================================");

            var repositoryResolver = provider.GetRequiredService<INamedServiceResolver>();

            var clockRepository = repositoryResolver.GetRequiredNamedService<IClock>("clock");
            Console.WriteLine($"Clock: {clockRepository.GetValue()}");

            var utcClockRepository = repositoryResolver.GetRequiredNamedService<IClock>("utcclock");
            Console.WriteLine($"Clock (UTC): {utcClockRepository.GetValue()}");

            Console.WriteLine();
        }
    }
}