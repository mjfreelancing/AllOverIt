﻿using System.Collections.Generic;

namespace AllOverIt.Aws.AppSync.Client.Subscription.Response
{
    public abstract class GraphqlResponseBase<TResponse>
    {
        public TResponse Data { get; set; }

        public IEnumerable<GraphqlErrorDetail> Errors { get; set; }
    }
}