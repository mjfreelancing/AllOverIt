using AllOverIt.Aws.AppSync.Client.Response;
using System;
using System.Collections.Generic;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    public interface IAppSyncSubscriptionRegistration : IAsyncDisposable
    {
        string Id { get; }
        IReadOnlyCollection<Exception> Exceptions { get; }
        IReadOnlyCollection<GraphqlErrorDetail> GraphqlErrors { get; }
        bool Success { get; }
    }
}