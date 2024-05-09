using AllOverIt.Assertion;
using AllOverIt.Aws.Cdk.AppSync.Resolvers;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Resolvers
{
    /// <summary>An abstract resolver attribute that contains a resolver type.</summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class GraphqlResolverAttribute : Attribute
    {
        /// <summary>The resolver type that provides the required request and response handler details.
        /// This type must inherit <see cref="IResolverRuntime"/>.</summary>
        public Type? ResolverType { get; }

        /// <summary>Constructor. Used when the resolver type is registered in code.</summary>
        protected GraphqlResolverAttribute()
        {
            // ResolverType will be null
        }

        /// <summary>Constructor.</summary>
        /// <param name="resolverType">The resolver type that provides the required request and response handler details.
        /// This type must inherit <see cref="IResolverRuntime"/>.</param>
        protected GraphqlResolverAttribute(Type resolverType)
        {
            ResolverType = resolverType.WhenNotNull(nameof(resolverType));
        }
    }
}