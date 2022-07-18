using AllOverIt.Assertion;
using AllOverIt.Aws.Cdk.AppSync.Attributes.DataSources;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Directives;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;
using AllOverIt.Aws.Cdk.AppSync.Exceptions;
using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Aws.Cdk.AppSync.Mapping;
using AllOverIt.Collections;
using AllOverIt.Extensions;
using Amazon.CDK.AWS.AppSync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync.Extensions
{
    internal static class MethodInfoExtensions
    {
        private static readonly IReadOnlyDictionary<SystemType, string> EmptyTypeNameOverrides = Dictionary.EmptyReadOnly<SystemType, string>();

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
            if (methodInfo.ReturnType.IsNullableType())
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
                parameterInfo.AssertParameterSchemaType(methodInfo);

                var requiredTypeInfo = parameterInfo.GetRequiredTypeInfo();

                // Passing null for the field name because we are not creating a graphql field type, it is an argument type.
                // The graphql fields are tracked for things like determining request/response mappings.
                var graphqlType = typeStore.GetGraphqlType(null, requiredTypeInfo, objectType => graphqlApi.AddType(objectType));

                args.Add(parameterInfo.Name.GetGraphqlName(), graphqlType);
            }

            return args;
        }

        public static void RegisterRequestResponseMappings(this MethodInfo methodInfo, string fieldMapping, MappingTemplates mappingTemplates, MappingTypeFactory mappingTypeFactory)
        {
            _ = fieldMapping.WhenNotNullOrEmpty(nameof(fieldMapping));

            var requestResponseMapping = GetRequestResponseMapping(methodInfo, mappingTypeFactory);

            // will be null if the mapping has already been populated (via code), or the factory will provide the information
            if (requestResponseMapping != null)
            {
                // fieldMapping includes the parent names too
                mappingTemplates.RegisterMappings(fieldMapping, requestResponseMapping.RequestMapping, requestResponseMapping.ResponseMapping);
            }
        }

        public static void AssertReturnSchemaType(this MethodInfo methodInfo, SystemType parentType)
        {
            // make sure TYPE schema types only have other TYPE types, and similarly for INPUT schema types.
            var parentSchemaType = parentType.GetGraphqlTypeDescriptor(EmptyTypeNameOverrides).SchemaType;
            var returnType = methodInfo.ReturnType;

            if (parentSchemaType is GraphqlSchemaType.Input or GraphqlSchemaType.Type)
            {
                var methodSchemaType = returnType.GetGraphqlTypeDescriptor(EmptyTypeNameOverrides).SchemaType;

                if (methodSchemaType is GraphqlSchemaType.Input or GraphqlSchemaType.Type)
                {
                    if (parentSchemaType != methodSchemaType)
                    {
                        throw new InvalidOperationException($"Expected '{returnType.FullName}.{methodInfo.Name}' to return a '{parentSchemaType}' type.");
                    }
                }
            }
        }

        public static Directive[] GetAuthDirectivesOrDefault(this MethodInfo methodInfo)
        {
            var attributes = methodInfo
                .GetCustomAttributes<AuthDirectiveBaseAttribute>(true)
                .AsReadOnlyCollection();

            return attributes.GetAuthDirectivesOrDefault();
        }

        private static IRequestResponseMapping GetRequestResponseMapping(MemberInfo memberInfo, MappingTypeFactory mappingTypeFactory)
        {
            var attribute = memberInfo.GetCustomAttribute<DataSourceAttribute>(true);

            if (attribute == null)
            {
                throw new InvalidOperationException($"Expected {memberInfo.DeclaringType!.Name}.{memberInfo.Name} to have a datasource attribute.");
            }

            // will be null if no type has been provided (assumes the mapping was added in code via MappingTemplates)
            return attribute.MappingType != null
                ? ( mappingTypeFactory).GetRequestResponseMapping(attribute.MappingType)
                : null;
        }
    }
}