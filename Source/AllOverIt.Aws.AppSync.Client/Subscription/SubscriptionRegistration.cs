using AllOverIt.Helpers;
using AllOverIt.Serialization.Abstractions;
using System;
using GraphqlRequestType = AllOverIt.Aws.AppSync.Client.Subscription.Constants.GraphqlRequestType;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    internal abstract class SubscriptionRegistration
    {
        internal class SubscriptionRequest : SubscriptionQueryMessage
        {
            public SubscriptionRequest(string id, SubscriptionQueryPayload payload)
            {
                Id = id.WhenNotNull(nameof(id));
                Type = GraphqlRequestType.Start;
                Payload = payload.WhenNotNull(nameof(payload));
            }
        }

        public string Id => Request.Id;
        public SubscriptionRequest Request { get; }

        public abstract void NotifyResponse(string message);

        protected SubscriptionRegistration(string id, SubscriptionQueryPayload payload)
        {
            _ = id.WhenNotNull(nameof(id));
            _ = payload.WhenNotNull(nameof(payload));

            Request = new SubscriptionRequest(id, payload);
        }
    }

    internal class SubscriptionRegistration<TResponse> : SubscriptionRegistration
    {
        private readonly IJsonSerializer _serializer;
        private Action<SubscriptionResponse<TResponse>> ResponseAction { get; }

        public SubscriptionRegistration(string id, SubscriptionQueryPayload payload, Action<SubscriptionResponse<TResponse>> responseAction,
            IJsonSerializer serializer)
            : base(id, payload)
        {
            ResponseAction = responseAction.WhenNotNull(nameof(responseAction));
            _serializer = serializer.WhenNotNull(nameof(serializer));
        }

        public override void NotifyResponse(string message)
        {
            var response = _serializer.DeserializeObject<WebSocketSubscriptionResponse<SubscriptionResponse<TResponse>>>(message);
            ResponseAction.Invoke(response.Payload);
        }
    }
}