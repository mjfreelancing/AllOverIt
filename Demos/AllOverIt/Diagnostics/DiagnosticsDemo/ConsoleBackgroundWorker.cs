using AllOverIt.Assertion;
using AllOverIt.Diagnostics.Breadcrumbs;
using AllOverIt.Diagnostics.Breadcrumbs.Extensions;
using AllOverIt.Extensions;
using AllOverIt.GenericHost;
using DiagnosticsDemo.Extensions;
using Microsoft.Extensions.Hosting;

namespace DiagnosticsDemo
{
    public sealed class ConsoleBackgroundWorker : BackgroundWorker
    {
        private readonly IBreadcrumbs _breadcrumbs;

        // Demo using IBreadcrumbs instead of ILogger (which can be printed on demand, such as in the case of an error)
        public ConsoleBackgroundWorker(IBreadcrumbs breadcrumbs, IHostApplicationLifetime applicationLifetime)
            : base(applicationLifetime)
        {
            _breadcrumbs = breadcrumbs.WhenNotNull();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _breadcrumbs.Add(this, "Waiting for everything to start");

            // ExecuteAsync() starts before OnStarted() fires, so this shows how to wait. The result indicates if the app has started
            // or not (gone into the Stopping state).
            var started = await WaitForStartup();

            if (!started)
            {
                _breadcrumbs.Add(this, "Failed to start");
                return;
            }

            _breadcrumbs.Add(this, "Now running");

            _breadcrumbs.Add(this, "Now running");

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);

            var tasks = Enumerable
                .Range(1, 10)
                .SelectToReadOnlyCollection(item =>
                {
                    return Task.Run(async () =>
                    {
                        try
                        {
                            var threadId = Task.CurrentId;

                            while (!linkedTokenSource.IsCancellationRequested)
                            {
                                _breadcrumbs.Add(this, $"Thread {threadId} is running", DateTime.Now);

                                await Task.Delay(500, cancellationToken).ConfigureAwait(false);
                            }
                        }
                        catch (TaskCanceledException)
                        {
                        }
                    }, linkedTokenSource.Token);
                });

            await Task.WhenAll(tasks);

            _breadcrumbs.AddIntArrayData(true);

            // The breadcrumbs will be logged by the main app after the user presses a key

            Console.WriteLine();
            Console.WriteLine("Background worker is done. Press a key to end the process.");
            Console.WriteLine();
        }

        protected override void OnStarted()
        {
            _breadcrumbs.Add("The background worker has started");
        }

        protected override void OnStopping()
        {
            _breadcrumbs.Add("The background worker is stopping");
        }

        protected override void OnStopped()
        {
            _breadcrumbs.Add("The background worker is done");
        }
    }
}