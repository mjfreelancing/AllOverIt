using AllOverIt.Async;
using AllOverIt.Extensions;
using AllOverIt.Wpf.Threading;
using AllOverIt.Wpf.Threading.Extensions;
using System.Runtime.ExceptionServices;
using System.Windows;

namespace ThreadBindingDemo
{
    public partial class MainWindow : Window
    {
        private bool _initialized;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs eventArgs)
        {
            base.OnActivated(eventArgs);

            if (_initialized)
            {
                return;
            }

            _initialized = true;

            var cts = new CancellationTokenSource();

            CreateBackgroundTask(cts.Token)
                .FireAndForget(exception =>
                {
                    cts.Cancel();

                    // Re-throw the exception on the main thread (capture to avoid a closure).
                    // Will be handled by the unhandled exception handler in app.cs
                    var edi = ExceptionDispatchInfo.Capture(exception);

                    UIThread.Invoke(() =>
                    {
                        edi.Throw();
                    });
                });
        }

        private Task CreateBackgroundTask(CancellationToken cancellationToken)
        {
            var count = 0;

            // Capture the context for the UI thread so we can switch back to the UI thread later
            var uiSynchronizationContext = SynchronizationContext.Current;

            LogMessage(uiSynchronizationContext, $"Starting on UI thread {Environment.CurrentManagedThreadId}{Environment.NewLine}");

            return RepeatingTask.StartAsync(async () =>
            {
                count++;

                // Now in the context of a worker thread - SynchronizationContext.Current will be null

                LogMessage(uiSynchronizationContext, $"Iteration #{count} - START (currently on thread Id {Environment.CurrentManagedThreadId})");

                try
                {
                    // Show the different ways of being able to switch to the UI thread.
                    switch (count)
                    {
                        case 1:
                            LogMessage(uiSynchronizationContext, "Switch to UI thread using 'UIThread.InvokeAsync()' - START");

                            // The code within InvokeAsync() will be scheduled on the UI thread. The use of
                            // ConfigureAwait() in this repeating task has no affect because there is no synchronization
                            // context to capture. This means the code executing after InvokeAsync() has completed
                            // will always be back on the same worker thread.
                            await UIThread.InvokeAsync(() =>
                            {
                                OutputTextBox.AppendText($"  => now on UI thread Id {Environment.CurrentManagedThreadId}{Environment.NewLine}");
                                return Task.CompletedTask;
                            }).ConfigureAwait(false);

                            LogMessage(uiSynchronizationContext, $"  Post InvokeAsync() using ConfigureAwait(false), now on thread Id {Environment.CurrentManagedThreadId}");

                            await UIThread.InvokeAsync(() =>
                            {
                                OutputTextBox.AppendText($"  => Switched back to UI thread Id {Environment.CurrentManagedThreadId}{Environment.NewLine}");
                                return Task.CompletedTask;
                            }).ConfigureAwait(true);

                            LogMessage(uiSynchronizationContext, $"  Post InvokeAsync() using ConfigureAwait(true), now on thread Id {Environment.CurrentManagedThreadId}");
                            LogMessage(uiSynchronizationContext, $"Switch to UI thread using 'UIThread.InvokeAsync()' - END{Environment.NewLine}");
                            break;

                        case 2:
                            LogMessage(uiSynchronizationContext, "Switch to UI thread using 'Dispatcher.SwitchToUiThread()' - START");

                            // There is no ConfigureAwait() - we are permanently switching to the UI thread
                            await Application.Current.Dispatcher.BindTo();

                            // This is safe, we are now on the UI thread
                            OutputTextBox.AppendText($"  => now on (and will remain on) UI thread Id {Environment.CurrentManagedThreadId}{Environment.NewLine}");

                            LogMessage(uiSynchronizationContext, $"Switch to UI thread using 'Dispatcher.SwitchToUiThread()' - END{Environment.NewLine}");
                            break;

                        case 3:
                            LogMessage(uiSynchronizationContext, "Switch to UI thread using 'UIThread.Bind()' - START");

                            // There is no ConfigureAwait() - we are permanently switching to the UI thread
                            await UIThread.BindToAsync();

                            // This is safe, we are now on the UI thread
                            OutputTextBox.AppendText($"  => now on (and will remain on) UI thread Id {Environment.CurrentManagedThreadId}{Environment.NewLine}");

                            LogMessage(uiSynchronizationContext, $"Switch to UI thread using 'UIThread.Bind()' - END{Environment.NewLine}");
                            break;

                        case 4:
                            LogMessage(uiSynchronizationContext, "Switch to UI thread using 'await uiSynchronizationContext' - START");

                            // There is no ConfigureAwait() - we are permanently switching to the UI thread
                            await uiSynchronizationContext;

                            // This is safe, we are now on the UI thread
                            OutputTextBox.AppendText($"  => now on (and will remain on) UI thread Id {Environment.CurrentManagedThreadId}{Environment.NewLine}");

                            LogMessage(uiSynchronizationContext, $"Switch to UI thread using 'await uiSynchronizationContext' - END{Environment.NewLine}");
                            break;

                        case 5:
                            LogMessage(uiSynchronizationContext, "Toggle using 'UIThread.InvokeAsync()' and 'UIThread.ContinueOnWorkerThread()' - START");

                            await UIThread.InvokeAsync(() =>
                            {
                                OutputTextBox.AppendText($"  => now on UI thread Id {Environment.CurrentManagedThreadId}{Environment.NewLine}");

                                return Task.CompletedTask;
                            });

                            LogMessage(uiSynchronizationContext, $"  Post InvokeAsync(), now on thread Id {Environment.CurrentManagedThreadId}");

                            await uiSynchronizationContext;

                            LogMessage(uiSynchronizationContext, $"  Switched to UI thread using 'await uiSynchronizationContext', now on thread Id {Environment.CurrentManagedThreadId}");

                            // NOTE: This will appear out of sequence in the logs because it is executed on a worker thread.
                            //       We could set up a signal to wait for it, but showing it out of sequence further validates it is running asynchronously.
                            UIThread.InvokeOnWorkerThread(() =>
                            {
                                LogMessage(uiSynchronizationContext, $"{Environment.NewLine} ** Switched to worker thread using 'UIThread.ContinueOnWorkerThread()' (iteration {count}), now on thread Id {Environment.CurrentManagedThreadId} (could be out of sequence){Environment.NewLine}{Environment.NewLine}");
                            });

                            await UIThread.InvokeAsync(() =>
                            {
                                LogMessage(uiSynchronizationContext, $"  Switched to UI thread using 'UIThread.InvokeAsync()', now on thread Id {Environment.CurrentManagedThreadId}");

                                return Task.CompletedTask;
                            });

                            LogMessage(uiSynchronizationContext, $"Toggle using 'UIThread.InvokeAsync()' and 'UIThread.ContinueOnWorkerThread()' - END{Environment.NewLine}");

                            break;

                        case 6:
                            throw new Exception($"Thrown after {count} iterations");
                    }

                    var isBound = UIThread.IsBound();
                    LogMessage(uiSynchronizationContext, $"Iteration {count} done, IsBound = {isBound}{Environment.NewLine}");

                }
                catch (Exception exception)
                {
                    LogMessage(uiSynchronizationContext, $"Exception: {exception.Message}");
                    throw;
                }
                finally
                {
                    LogMessage(uiSynchronizationContext, $"Iteration #{count} - END (currently on thread Id {Environment.CurrentManagedThreadId}){Environment.NewLine}{Environment.NewLine}");
                }
            },
            new RepeatingTaskOptions
            {
                RepeatDelay = TimeSpan.FromMilliseconds(250)
            },
            cancellationToken);
        }

        private void LogMessage(SynchronizationContext uiSynchronizationContext, string message)
        {
            if (UIThread.IsBound())
            {
                OutputTextBox.AppendText($"{message}{Environment.NewLine}");
            }
            else
            {
                uiSynchronizationContext.Post(_ =>
                {
                    OutputTextBox.AppendText($"{message}{Environment.NewLine}");
                }, null);
            }
        }
    }
}
