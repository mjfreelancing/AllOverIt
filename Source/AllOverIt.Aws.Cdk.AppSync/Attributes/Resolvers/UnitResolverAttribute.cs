using AllOverIt.Assertion;
using System;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Resolvers
{
    public sealed class UnitResolverAttribute : GraphQlResolverAttribute
    {
        public string DataSourceName { get; }

        // Used when the resolver type is registered in code
        public UnitResolverAttribute(string dataSourceName)
        {
            DataSourceName = dataSourceName.WhenNotNullOrEmpty(nameof(dataSourceName));
        }

        public UnitResolverAttribute(Type resolverType, string dataSourceName)
            : base(resolverType)
        {
            DataSourceName = dataSourceName.WhenNotNullOrEmpty(nameof(dataSourceName));
        }
    }
}