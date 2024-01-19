using AllOverIt.Assertion;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Async
{
    /// <summary>Provides support for executing an action asynchronously in a background task with automatic
    /// cancellation when disposed.</summary>
    public sealed class BackgroundTask : IAsyncDisposable
    {
        private bool _isDisposing = false;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly Task _task;

        /// <summary>Constructor</summary>
        /// <param name="action">The action to invoke asynchronously in a background task. Use the <paramref name="cancellationToken"/>
        /// to detect if the action should be cancelled.</param>
        /// <param name="cancellationToken">An optional cancellation token that can cancel the task.</param>
        public BackgroundTask(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
            : this(action, _ => false, cancellationToken)
        {
        }

        /// <summary>Constructor</summary>
        /// <param name="action">The action to invoke asynchronously in a background task. Use the <paramref name="cancellationToken"/>
        /// to detect if the action should be cancelled.</param>
        /// <param name="exceptionHandler">An exception handler that is invoked if an exception is raised. The handler must return
        /// <see langword="true"/> if the exception is handled. If the handler returns <see langword="false"/> the exception will be re-thrown.</param>
        /// <param name="cancellationToken">An optional cancellation token that will cancel the task if cancelled.</param>
        public BackgroundTask(Func<CancellationToken, Task> action, Func<ExceptionDispatchInfo, bool> exceptionHandler, CancellationToken cancellationToken = default)
        {
            _ = action.WhenNotNull(nameof(action));
            _ = exceptionHandler.WhenNotNull(nameof(exceptionHandler));

            var taskToken = CreateTaskCancellationToken(cancellationToken);

            _task = Task.Factory
                .StartNew(async () =>
                {
                    // ConfigureAwait() isn't strictly required here as there is no synchronization context,
                    // but it keeps all code consistent.
                    await InvokeActionAsync(action, exceptionHandler, taskToken).ConfigureAwait(false);
                }, taskToken)
                .Unwrap();
        }

        /// <summary>Constructor</summary>
        /// <param name="action">The action to invoke asynchronously in a background task. Use the <paramref name="cancellationToken"/>
        /// to detect if the action should be cancelled.</param>
        /// <param name="creationOptions">A <see cref="TaskCreationOptions"/> value that controls the behavior of the created task.</param>
        /// <param name="scheduler">The <see cref="TaskScheduler"/> that is used to schedule the created task.</param>
        /// <param name="cancellationToken">An optional cancellation token that will cancel the task if cancelled.</param>
        public BackgroundTask(Func<CancellationToken, Task> action, TaskCreationOptions creationOptions, TaskScheduler scheduler,
            CancellationToken cancellationToken = default)
            : this(action, creationOptions, scheduler, _ => false, cancellationToken)
        {
        }

        /// <summary>Constructor</summary>
        /// <param name="action">The action to invoke asynchronously in a background task. Use the <paramref name="cancellationToken"/>
        /// to detect if the action should be cancelled.</param>
        /// <param name="creationOptions">A <see cref="TaskCreationOptions"/> value that controls the behavior of the created task.</param>
        /// <param name="scheduler">The <see cref="TaskScheduler"/> that is used to schedule the created task.</param>
        /// <param name="exceptionHandler">An exception handler that is invoked if an exception is raised. The handler must return
        /// <see langword="true"/> if the exception is handled. If the handler returns <see langword="false"/> the exception will be re-thrown.</param>
        /// <param name="cancellationToken">An optional cancellation token that will cancel the task if cancelled.</param>
        public BackgroundTask(Func<CancellationToken, Task> action, TaskCreationOptions creationOptions, TaskScheduler scheduler,
            Func<ExceptionDispatchInfo, bool> exceptionHandler, CancellationToken cancellationToken = default)
        {
            _ = action.WhenNotNull(nameof(action));
            _ = exceptionHandler.WhenNotNull(nameof(exceptionHandler));

            var taskToken = CreateTaskCancellationToken(cancellationToken);

            _task = Task.Factory
                .StartNew(async () =>
                {
                    // ConfigureAwait() isn't strictly required here as there is no synchronization context,
                    // but it keeps all code consistent.
                    await InvokeActionAsync(action, exceptionHandler, taskToken).ConfigureAwait(false);
                }, taskToken, creationOptions, scheduler)
                .Unwrap();
        }

        /// <summary>An implicit operator that returns a <see cref="BackgroundTask"/> as a <see cref="Task"/>.</summary>
        /// <param name="backgroundTask">The <see cref="BackgroundTask"/> to implicitly convert to a <see cref="Task"/>.</param>
        public static implicit operator Task(BackgroundTask backgroundTask)
        {
            return backgroundTask?._task;
        }

        /// <summary>Gets an awaiter used to await this <see cref="BackgroundTask"/>.</summary>
        /// <returns>An awaiter instance.</returns>
        public TaskAwaiter GetAwaiter()
        {
            return _task.GetAwaiter();
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            _isDisposing = true;

            try
            {
                if (!_task.IsCompleted)
                {
                    if (!_task.IsFaulted)
                    {
                        _cancellationTokenSource.Cancel();
                    }

                    // This could fault
                    await _task.ConfigureAwait(false);
                }
            }
            catch
            {
                // Don't allow an exception to be thrown from here
            }
            finally
            {
                _cancellationTokenSource.Dispose();
            }
        }

        private CancellationToken CreateTaskCancellationToken(CancellationToken cancellationToken = default)
        {
            return cancellationToken == default
                ? _cancellationTokenSource.Token
                : CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken).Token;
        }

        private async Task InvokeActionAsync(Func<CancellationToken, Task> action, Func<ExceptionDispatchInfo, bool> exceptionHandler,
            CancellationToken cancellationToken)
        {
            try
            {
                await action.Invoke(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (_isDisposing)
                {
                    return;
                }

                if (exceptionHandler is not null)
                {
                    var edi = ExceptionDispatchInfo.Capture(exception);

                    if (exceptionHandler.Invoke(edi))
                    {
                        return;
                    }
                }

                throw;
            }
        }
    }
}
