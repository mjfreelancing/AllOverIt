using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Exceptions;
using AllOverIt.Aws.Cdk.AppSync.Mapping;
using AllOverIt.Extensions;
using AllOverIt.Helpers;
using Amazon.CDK.AWS.AppSync;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AllOverIt.Aws.Cdk.AppSync.Extensions
{
    internal static class MethodInfoExtensions
    {
        public static RequiredTypeInfo GetRequiredTypeInfo(this MethodInfo methodInfo)
        {
            return new RequiredTypeInfo(methodInfo);
        }

        public static bool IsGqlTypeRequired(this MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttribute<SchemaTypeRequiredAttribute>(true) != null;
        }

        public static bool IsGqlArrayRequired(this MethodInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<SchemaArrayRequiredAttribute>(true) != null;
        }

        public static void AssertReturnTypeIsNotNullable(this MethodInfo methodInfo)
        {
            if (methodInfo.ReturnType.IsGenericNullableType())
            {
                throw new SchemaException($"{methodInfo.DeclaringType!.Name}.{methodInfo.Name} has a nullable return type. The presence of {nameof(SchemaTypeRequiredAttribute)} " +
                                           "is used to declare a property as required, and its absence makes it optional.");
            }
        }

        public static IDictionary<string, GraphqlType> GetMethodArgs(this MethodInfo methodInfo, GraphqlApi graphqlApi, GraphqlTypeStore typeStore)
        {
            var parameters = methodInfo.GetParameters();

            if (!parameters.Any())
            {
                return null;
            }

            var args = new Dictionary<string, GraphqlType>();

            foreach (var parameterInfo in parameters)
            {
                parameterInfo.AssertParameterTypeIsNotNullable();

                var requiredTypeInfo = parameterInfo.GetRequiredTypeInfo();

                // Passing null for the field name because we are not creating a graphql field type, it is an argument type.
                // The graphql fields are tracked for things like determining request/response mappings.
                var graphqlType = typeStore.GetGraphqlType(null, requiredTypeInfo, objectType => graphqlApi.AddType(objectType));

                args.Add(parameterInfo.Name.GetGraphqlName(), graphqlType);
            }

            return args;
        }

        public static void RegisterRequestResponseMappings(this MethodInfo methodInfo, string fieldMapping, MappingTemplates mappingTemplates)
        {
            _ = fieldMapping.WhenNotNullOrEmpty(nameof(fieldMapping));

            var mappingAttribute = methodInfo.GetRequestResponseMapping();

            if (mappingAttribute != null)
            {
                var mapping = mappingAttribute.MappingType;

                // fieldMapping includes the parent names too
                mappingTemplates.RegisterMappings(fieldMapping, mapping.RequestMapping, mapping.ResponseMapping);
            }
        }
    }
}