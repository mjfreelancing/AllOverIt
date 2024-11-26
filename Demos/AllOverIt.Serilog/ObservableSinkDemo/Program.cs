using AllOverIt.GenericHost;
using AllOverIt.Serilog.Enrichers.ThreadId;
using AllOverIt.Serilog.Extensions;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Templates;

namespace ObservableSinkDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).RunConsoleAsync(options => options.SuppressStatusMessages = true);
        }

        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-5.0
        // If the app uses Entity Framework Core, don't change the name or signature of the CreateHostBuilder method.
        // The Entity Framework Core tools expect to find a CreateHostBuilder method that configures the host without
        // running the app. For more information, see https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dbcontext-creation
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            /*
                This demo sets up a console filter to only log messages that contain '13'. It then outputs all
                messages that we captured by the Serilog sink to the console (using Console.WriteLine). The output
                is custom formatter, and looks similar to this:

                    Filtered to console: 2024-02-08 20:03:39.044 +11:00 [INF] (Thread 20) 138
                    Filtered to console: 2024-02-08 20:03:38.914 +11:00 [INF] (Thread 5) 113
                    Filtered to console: 2024-02-08 20:03:38.948 +11:00 [INF] (Thread 13) 134
                    Filtered to console: 2024-02-08 20:03:38.986 +11:00 [INF] (Thread 15) 135
                    Filtered to console: 2024-02-08 20:03:38.947 +11:00 [INF] (Thread 12) 132
                    Filtered to console: 2024-02-08 20:03:38.915 +11:00 [INF] (Thread 11) 130
                    Filtered to console: 2024-02-08 20:03:38.947 +11:00 [INF] (Thread 14) 133
                    Filtered to console: 2024-02-08 20:03:38.915 +11:00 [INF] (Thread 10) 131
                    Filtered to console: 2024-02-08 20:03:38.986 +11:00 [INF] (Thread 16) 136
                    Filtered to console: 2024-02-08 20:03:38.986 +11:00 [INF] (Thread 17) 137
                    Filtered to console: 2024-02-08 20:03:38.983 +11:00 [INF] (Thread 7) 139
             */

            return GenericHost
                .CreateConsoleHostBuilder<App>(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSerilogObservable();        // defaults to Scoped lifetime
                })
                .UseSerilog((hostBuilderContext, serviceProvider, configuration) =>
                {
                    configuration.Enrich.With<ThreadIdEnricher>();

                    var observableSink = serviceProvider.GetObservableSink();

                    configuration.WriteTo.Observable(observableSink);

                    // Using a filter expression to limit the console logs to those containing '13'.
                    // https://github.com/serilog/serilog-expressions
                    configuration.WriteTo.Console(new ExpressionTemplate("{#if Contains(@m, '13')}Filtered to console: {@t:yyyy-MM-dd HH:mm:ss.fff zzz} [{@l:u3}] (Thread {ThreadId}) {@m}\n{@x}{#end}"));
                });
        }
    }

}