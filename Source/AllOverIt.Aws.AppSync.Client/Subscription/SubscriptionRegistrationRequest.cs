using AllOverIt.Assertion;
using AllOverIt.Aws.AppSync.Client.Response;
using AllOverIt.Serialization.Json.Abstractions;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    /// <summary>Contains information about a registered subscription.</summary>
    internal abstract class SubscriptionRegistrationRequest
    {
        internal class SubscriptionRequest : SubscriptionQueryMessage
        {
            public SubscriptionRequest(string id, SubscriptionQueryPayload payload)
            {
                Id = id.WhenNotNull(nameof(id));
                Type = ProtocolMessage.Request.Start;
                Payload = payload.WhenNotNull(nameof(payload));
            }
        }

        /// <summary>The subscription identifier.</summary>
        public string Id => Request.Id!;

        /// <summary>The subscription message used for registration with AppSync.</summary>
        public SubscriptionRequest Request { get; }

        /// <summary>Indicates if the subscription is currently registered with AppSync.</summary>
        public bool IsSubscribed { get; set; }

        /// <summary>This method is called when a message is received from AppSync that needs to be decoded into a strongly typed response.</summary>
        /// <param name="message">The response data as a serialized string.</param>
        public abstract void NotifyResponse(string message);

        /// <summary>Constructor.</summary>
        /// <param name="id">The subscription identifier.</param>
        /// <param name="payload">The registration payload to be sent to AppSync.</param>
        protected SubscriptionRegistrationRequest(string id, SubscriptionQueryPayload payload)
        {
            _ = id.WhenNotNullOrEmpty(nameof(id));
            _ = payload.WhenNotNull(nameof(payload));

            Request = new SubscriptionRequest(id, payload);
        }
    }

    /// <summary>A strongly typed wrapper for a subscription registration.</summary>
    /// <typeparam name="TResponse">The response type to be populated when the subscription receives data.</typeparam>
    internal sealed class SubscriptionRegistrationRequest<TResponse> : SubscriptionRegistrationRequest
    {
        private readonly IJsonSerializer _serializer;
        private Action<GraphqlSubscriptionResponse<TResponse>> ResponseAction { get; }

        /// <summary>Constructor.</summary>
        /// <param name="id">The subscription identifier.</param>
        /// <param name="payload">The registration payload to be sent to AppSync.</param>
        /// <param name="responseAction">The action to invoke when the subscription receives data.</param>
        /// <param name="serializer">The serializer to use when deserializing the received content. See 'AllOverIt.Serialization.NewtonsoftJson'
        /// and 'AllOverIt.Serialization.SystemTextJson' for suitable implementations.</param>
        public SubscriptionRegistrationRequest(string id, SubscriptionQueryPayload payload, Action<GraphqlSubscriptionResponse<TResponse>> responseAction,
            IJsonSerializer serializer)
            : base(id, payload)
        {
            ResponseAction = responseAction.WhenNotNull(nameof(responseAction));
            _serializer = serializer.WhenNotNull(nameof(serializer));
        }

        /// <summary>Deserializes the received message into a strongly-typed response.</summary>
        /// <param name="message">The response data as a serialized string.</param>
        public override void NotifyResponse(string message)
        {
            var response = _serializer.DeserializeObject<WebSocketSubscriptionResponse<GraphqlSubscriptionResponse<TResponse>>>(message);
            ResponseAction.Invoke(response.Payload);
        }
    }
}