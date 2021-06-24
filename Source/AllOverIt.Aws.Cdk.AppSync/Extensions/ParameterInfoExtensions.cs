using AllOverIt.Aws.Cdk.AppSync.Attributes;
using System.Reflection;

namespace AllOverIt.Aws.Cdk.AppSync.Extensions
{
    internal static class ParameterInfoExtensions
    {
        public static bool IsGqlTypeRequired(this ParameterInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<SchemaTypeRequiredAttribute>(true) != null;
        }

        public static bool IsGqlArrayRequired(this ParameterInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<SchemaArrayRequiredAttribute>(true) != null;
        }
    }
}