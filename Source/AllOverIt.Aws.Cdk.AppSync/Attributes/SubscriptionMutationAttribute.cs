using AllOverIt.Aws.Cdk.AppSync.Extensions;
using AllOverIt.Extensions;
using AllOverIt.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes
{
    /// <summary>Apply to a subscription to indicate which mutations will trigger the subscription notifications.</summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SubscriptionMutationAttribute : Attribute
    {
        /// <summary>One or more mutations that will trigger the subscription notifications.</summary>
        public IEnumerable<string> Mutations { get; }

        /// <summary>Constructor.</summary>
        /// <param name="mutations">One or more mutations that will trigger the subscription notifications.</param>
        public SubscriptionMutationAttribute(params string[] mutations)
        {
            Mutations = mutations.WhenNotNullOrEmpty(nameof(mutations))
                .Select(item => item.GetGraphqlName())
                .AsReadOnlyCollection();
        }
    }
}