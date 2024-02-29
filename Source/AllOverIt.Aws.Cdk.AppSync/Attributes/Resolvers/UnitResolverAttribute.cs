using AllOverIt.Assertion;
using System;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Resolvers
{
    public sealed class UnitResolverAttribute : GraphQlResolverAttribute
    {
        public string DataSourceId { get; }

        // Used when the resolver type is registered in code
        public UnitResolverAttribute(string dataSourceId)
        {
            DataSourceId = dataSourceId.WhenNotNullOrEmpty(nameof(dataSourceId));
        }

        public UnitResolverAttribute(Type resolverType, string dataSourceId)
            : base(resolverType)
        {
            DataSourceId = dataSourceId.WhenNotNullOrEmpty(nameof(dataSourceId));
        }
    }
}