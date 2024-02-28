using AllOverIt.Assertion;
using AllOverIt.Aws.Cdk.AppSync.Resolvers;
using System;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Resolvers
{
    /// <summary>An abstract resolver attribute that contains a resolver type.</summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class GraphQlResolverAttribute : Attribute
    {
        /// <summary>The resolver type that provides the required request and response mapping details. This type must inherit <see cref="IResolverRuntime"/>.</summary>
        public Type ResolverType { get; }

        // Used when the resolver type is registered in code - ResolverType will be null
        protected GraphQlResolverAttribute()
        {
        }

        protected GraphQlResolverAttribute(Type resolverType)
        {
            ResolverType = resolverType.WhenNotNull(nameof(resolverType));
        }
    }
}