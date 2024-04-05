using AllOverIt.Assertion;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Directives;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Resolvers;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;
using AllOverIt.Aws.Cdk.AppSync.Exceptions;
using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Aws.Cdk.AppSync.Resolvers;
using AllOverIt.Collections;
using AllOverIt.Extensions;
using Cdklabs.AwsCdkAppsyncUtils;
using System.Collections.Generic;
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

        public static IDictionary<string, GraphqlType> GetMethodArgs(this MethodInfo methodInfo, GraphqlSchema schema, GraphqlTypeStore typeStore)
        {
            var parameters = methodInfo.GetParameters();

            if (parameters.Length == 0)
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
                // The graphql fields are tracked for things like determining resolver request/response handlers.
                var graphqlType = typeStore.GetGraphqlType(null, requiredTypeInfo, objectType => schema.AddType(objectType));

                args.Add(parameterInfo.Name.GetGraphqlName(), graphqlType);
            }

            return args;
        }

        public static void RegisterResolver(this MethodInfo methodInfo, string fieldMapping, ResolverRegistry resolverRegistry, ResolverFactory resolverFactory)
        {
            _ = fieldMapping.WhenNotNullOrEmpty(nameof(fieldMapping));

            // Will be null if the resolver has already been populated (via code), or the factory will provide the information,
            // or it isn't required (such as Subscriptions).
            var resolver = GetResolverRuntime(methodInfo, resolverFactory);

            if (resolver is not null)
            {
                resolverRegistry.RegisterResolver(fieldMapping, resolver);
            }
        }

        public static void AssertReturnSchemaType(this MethodInfo methodInfo, SystemType parentType)
        {
            // Make sure TYPE schema types only have other TYPE types, and similarly for INPUT schema types.
            var parentSchemaType = parentType.GetGraphqlTypeDescriptor(EmptyTypeNameOverrides).SchemaType;
            var returnType = methodInfo.ReturnType;

            if (parentSchemaType is GraphqlSchemaType.Input or GraphqlSchemaType.Type)
            {
                var methodSchemaType = returnType.GetGraphqlTypeDescriptor(EmptyTypeNameOverrides).SchemaType;

                if (methodSchemaType is GraphqlSchemaType.Input or GraphqlSchemaType.Type)
                {
                    if (parentSchemaType != methodSchemaType)
                    {
                        throw new SchemaException($"Expected '{returnType.FullName}.{methodInfo.Name}' to return a '{parentSchemaType}' type.");
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

        private static IResolverRuntime GetResolverRuntime(MethodInfo memberInfo, ResolverFactory resolverFactory)
        {
            var resolverAttribute = memberInfo.GetCustomAttribute<GraphqlResolverAttribute>(true);

            // Will be null if no resolver type has been provided (assumes it was added code-first rather than on an attribute)
            if (resolverAttribute?.ResolverType is not null)
            {
                return resolverFactory.GetResolverRuntime(resolverAttribute.ResolverType);
            }

            return null;
        }
    }
}