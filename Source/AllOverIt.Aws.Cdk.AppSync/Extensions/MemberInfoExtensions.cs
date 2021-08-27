using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Aws.Cdk.AppSync.Mapping;
using AllOverIt.Extensions;
using Amazon.CDK.AWS.AppSync;
using System;
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

        public static IRequestResponseMapping GetRequestResponseMapping(this MemberInfo memberInfo)
        {
            var attribute = memberInfo.GetCustomAttribute<DataSourceAttribute>(true);

            if (attribute == null)
            {
                throw new InvalidOperationException($"Expected {memberInfo.DeclaringType!.Name}.{memberInfo.Name} to have a datasource attribute");
            }

            return attribute.MappingType;
        }

        public static string GetFieldName(this MemberInfo memberInfo, string parentName)
        {
            return parentName.IsNullOrEmpty()
                ? memberInfo.Name
                : $"{parentName}.{memberInfo.Name}";
        }
    }
}