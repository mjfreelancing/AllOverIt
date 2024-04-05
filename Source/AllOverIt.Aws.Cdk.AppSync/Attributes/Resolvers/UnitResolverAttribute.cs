using AllOverIt.Assertion;
using AllOverIt.Aws.Cdk.AppSync.Resolvers;
using System;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Resolvers
{
    /// <summary>A unit resolver attribute that contains a resolver type.</summary>
    public sealed class UnitResolverAttribute : GraphqlResolverAttribute
    {
        /// <summary>The datasource name.</summary>
        public string DataSourceName { get; }

        /// <summary>Constructor. Used when the resolver type is registered in code.</summary>
        /// <param name="dataSourceName">The datasource name.</param>
        public UnitResolverAttribute(string dataSourceName)
        {
            DataSourceName = dataSourceName.WhenNotNullOrEmpty(nameof(dataSourceName));
        }

        /// <summary>Constructor.</summary>
        /// <param name="resolverType">The resolver type that provides the required request and response handler details.
        /// This type must inherit <see cref="IResolverRuntime"/>.</param>
        /// <param name="dataSourceName">The datasource name.</param>
        public UnitResolverAttribute(Type resolverType, string dataSourceName)
            : base(resolverType)
        {
            DataSourceName = dataSourceName.WhenNotNullOrEmpty(nameof(dataSourceName));
        }
    }
}