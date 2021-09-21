namespace AllOverIt.Aws.AppSync.Client.Response
{
    /// <summary>Contains response data for a subscription request when the associated mutation has been invoked.</summary>
    /// <typeparam name="TResponse">The subscription response type.</typeparam>
    public sealed record GraphqlSubscriptionResponse<TResponse> : GraphqlResponseBase<TResponse>
    {
    }
}