using AllOverIt.Aws.Cdk.AppSync.Attributes.Resolvers;
using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Extensions;
using Amazon.CDK.AWS.AppSync;
using System.Reflection;

namespace AllOverIt.Aws.Cdk.AppSync.Extensions
{
    internal static class MemberInfoExtensions
    {
        public static BaseDataSource? GetDataSource(this MemberInfo memberInfo, DataSourceFactory dataSourceFactory)
        {
            var resolverAttribute = memberInfo.GetCustomAttribute<GraphqlResolverAttribute>(true);

            if (resolverAttribute is not null)
            {
                return dataSourceFactory.CreateDataSource(resolverAttribute);
            }

            return null;
        }

        public static string GetFieldName(this MemberInfo memberInfo, string? parentName)
        {
            return parentName.IsNullOrEmpty()
                ? memberInfo.Name
                : $"{parentName}.{memberInfo.Name}";
        }
    }
}