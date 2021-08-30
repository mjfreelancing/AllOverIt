using AllOverIt.Aws.Cdk.AppSync.Attributes.Datasources;
using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Extensions;
using Amazon.CDK.AWS.AppSync;
using System.Reflection;

namespace AllOverIt.Aws.Cdk.AppSync.Extensions
{
    internal static class MemberInfoExtensions
    {
        public static BaseDataSource GetDataSource(this MemberInfo memberInfo, DataSourceFactory dataSourceFactory)
        {
            var attribute = memberInfo.GetCustomAttribute<DataSourceAttribute>(true);

            return attribute == null
                ? null
                : dataSourceFactory.CreateDataSource(attribute);
        }

        public static string GetFieldName(this MemberInfo memberInfo, string parentName)
        {
            return parentName.IsNullOrEmpty()
                ? memberInfo.Name
                : $"{parentName}.{memberInfo.Name}";
        }
    }
}