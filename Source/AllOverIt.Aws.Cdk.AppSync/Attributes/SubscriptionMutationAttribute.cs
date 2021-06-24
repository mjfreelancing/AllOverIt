using AllOverIt.Aws.Cdk.AppSync.Extensions;
using AllOverIt.Extensions;
using AllOverIt.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes
{
    // Used to indicate which mutations will trigger the subscription notification
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SubscriptionMutationAttribute : Attribute
    {
        public IEnumerable<string> Mutations { get; }

        public SubscriptionMutationAttribute(params string[] mutations)
        {
            Mutations = mutations.WhenNotNullOrEmpty(nameof(mutations))
                .Select(item => item.GetGraphqlName())
                .AsReadOnlyCollection();
        }
    }
}