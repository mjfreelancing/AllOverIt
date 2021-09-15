﻿using AllOverIt.Aws.AppSync.Client.Subscription.Response;
using AllOverIt.Helpers;
using AllOverIt.Serialization.Abstractions;
using System;

namespace AllOverIt.Aws.AppSync.Client.Subscription.Request
{
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

        public string Id => Request.Id;
        public SubscriptionRequest Request { get; }
        public bool IsSubscribed { get; set; }

        public abstract void NotifyResponse(string message);

        protected SubscriptionRegistrationRequest(string id, SubscriptionQueryPayload payload)
        {
            _ = id.WhenNotNull(nameof(id));
            _ = payload.WhenNotNull(nameof(payload));

            Request = new SubscriptionRequest(id, payload);
        }
    }

    internal sealed class SubscriptionRegistrationRequest<TResponse> : SubscriptionRegistrationRequest
    {
        private readonly IJsonSerializer _serializer;
        private Action<SubscriptionResponse<TResponse>> ResponseAction { get; }

        public SubscriptionRegistrationRequest(string id, SubscriptionQueryPayload payload, Action<SubscriptionResponse<TResponse>> responseAction,
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