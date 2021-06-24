using AllOverIt.Aws.Cdk.AppSync.Attributes;
using System.Reflection;

namespace AllOverIt.Aws.Cdk.AppSync.Extensions
{
    internal static class PropertyInfoExtensions
    {
        public static bool IsGqlTypeRequired(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<SchemaTypeRequiredAttribute>(true) != null;
        }

        public static bool IsGqlArrayRequired(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<SchemaArrayRequiredAttribute>(true) != null;
        }

        public static GraphqlSchemaTypeDescriptor GetGraphqlPropertyDescriptor(this PropertyInfo propertyInfo)
        {
            var type = propertyInfo.PropertyType;

            var elementType = type.IsArray
                ? type.GetElementType()
                : type;

            return elementType.GetGraphqlTypeDescriptor(propertyInfo);
        }
    }
}