using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Factories;
using Amazon.CDK.AWS.AppSync;
using System.Reflection;

namespace AllOverIt.Aws.Cdk.AppSync.Extensions
{
    internal static class MemberInfoExtensions
    {
        public static BaseDataSource GetDataSource(this MemberInfo methodInfo, IDataSourceFactory dataSourceFactory)
        {
            var attribute = methodInfo.GetCustomAttribute<DataSourceAttribute>(true);

            return attribute == null
                ? null
                : dataSourceFactory.CreateDataSource(attribute);
        }

        public static string GetFunctionName(this MemberInfo methodInfo)
        {
            var attribute = methodInfo.GetCustomAttribute<DataSourceAttribute>(true);

            if (attribute is LambdaDataSourceAttribute lambdaDataSourceAttribute)
            {
                return lambdaDataSourceAttribute.FunctionName;
            }

            return null;
        }
    }
}