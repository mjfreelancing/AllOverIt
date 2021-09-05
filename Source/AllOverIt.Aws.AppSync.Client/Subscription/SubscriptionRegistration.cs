using AllOverIt.Helpers;
using Newtonsoft.Json;
using System;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    internal class SubscriptionRegistration<TResponse> : SubscriptionRegistration
    {
        private Action<SubscriptionResponse<TResponse>> ResponseAction { get; }

        public SubscriptionRegistration(SubscriptionQueryPayload payload, Action<SubscriptionResponse<TResponse>> responseAction)
            : base(payload)
        {
            ResponseAction = responseAction.WhenNotNull(nameof(responseAction));
        }

        public override void NotifyResponse(string message)
        {
            // todo: add serializer - how to do with other framework
            var response = JsonConvert.DeserializeObject<WebSocketSubscriptionResponse<SubscriptionResponse<TResponse>>>(message);

            ResponseAction.Invoke(response.Payload);
        }
    }

    internal abstract class SubscriptionRegistration
    {
        internal class SubscriptionRequest : SubscriptionQueryMessage
        {
            public SubscriptionRequest(SubscriptionQueryPayload payload)
            {
                Id = $"{Guid.NewGuid():N}";
                Type = "start";
                Payload = payload.WhenNotNull(nameof(payload));
            }
        }

        public string Id => Request.Id;
        public SubscriptionRequest Request { get; }

        public abstract void NotifyResponse(string message);

        protected SubscriptionRegistration(SubscriptionQueryPayload payload)
        {
            _ = payload.WhenNotNull(nameof(payload));

            Request = new SubscriptionRequest(payload);
        }
    }
}