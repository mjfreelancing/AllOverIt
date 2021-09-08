using System;
using System.Collections.Generic;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    public interface IAppSubscriptionRegistration : IAsyncDisposable
    {
        string Id { get; }
        IReadOnlyCollection<Exception> Exceptions { get; }
        IReadOnlyCollection<GraphqlErrorDetail> GraphqlErrors { get; }
        bool Success { get; }
    }
}