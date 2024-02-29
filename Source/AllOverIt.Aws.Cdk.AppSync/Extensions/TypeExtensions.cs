using AllOverIt.Assertion;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Directives;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;
using AllOverIt.Aws.Cdk.AppSync.Exceptions;
using AllOverIt.Extensions;
using AllOverIt.Reflection;
using Cdklabs.AwsCdkAppsyncUtils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync.Extensions
{
    internal static class TypeExtensions
    {
        public static SystemType GetElementTypeIfArray(this SystemType type)
        {
            return type.IsArray
                ? type.GetElementType()
                : type;
        }

        private static bool TryGetSchemaAttribute<TType>(TypeInfo typeInfo, out SchemaTypeBaseAttribute attribute)
            where TType : SchemaTypeBaseAttribute
        {
            var schemaTypeAttributes = typeInfo
                .GetCustomAttributes<TType>(true)
                .AsReadOnlyCollection();

            if (schemaTypeAttributes.Count != 0)
            {
                if (schemaTypeAttributes.Count > 1)
                {
                    throw new SchemaException($"'{typeInfo.Name}' contains more than one schema type attribute.");
                }

                attribute = schemaTypeAttributes.Single();
                return true;
            }

            attribute = default;
            return false;
        }

        public static GraphqlSchemaTypeDescriptor GetGraphqlTypeDescriptor(this SystemType type, IReadOnlyDictionary<SystemType, string> typeNameOverrides)
        {
            _ = typeNameOverrides.WhenNotNull();

            var elementType = type.GetElementTypeIfArray();

            _ = typeNameOverrides.TryGetValue(elementType, out var typeNameOverride);

            var typeInfo = elementType!.GetTypeInfo();

            // If the type is an enum explicitly look for the lack of an attribute
            var isEnum = elementType.IsEnum || elementType.IsEnrichedEnum();

            if (isEnum && !TryGetSchemaAttribute<SchemaTypeBaseAttribute>(typeInfo, out _))
            {
                return typeNameOverride.IsNullOrEmpty()
                    ? new GraphqlSchemaTypeDescriptor(elementType, GraphqlSchemaType.Enum, elementType.Name)
                    : new GraphqlSchemaTypeDescriptor(elementType, GraphqlSchemaType.Enum, typeNameOverride);
            }

            // SchemaTypeAttribute indicates if this is a scalar, enum, interface, or input type (cannot be on an array)
            if (TryGetSchemaAttribute<SchemaTypeBaseAttribute>(typeInfo, out var schemaTypeAttribute))
            {
                var schemaTypeName = GetNamespaceBasedName(type.Namespace, schemaTypeAttribute.ExcludeNamespacePrefix, schemaTypeAttribute.Name);

                if (typeNameOverride.IsNotNullOrEmpty() && schemaTypeName.IsNotNullOrEmpty())
                {
                    throw new SchemaException($"The type {type.GetFriendlyName()} has an attribute based name ({schemaTypeName}) and an override ({typeNameOverride}). Only one is allowed.");
                }

                if (schemaTypeName.IsNullOrEmpty())
                {
                    schemaTypeName = typeNameOverride.IsNotNullOrEmpty()
                        ? typeNameOverride
                        : typeInfo.Name;
                }

                return new GraphqlSchemaTypeDescriptor(elementType, schemaTypeAttribute.GraphqlSchemaType, schemaTypeName);
            }

            // not expecting class types to be used, but check anyway
            if (elementType != CommonTypes.StringType && (elementType.IsClass || elementType.IsInterface))
            {
                var typeDescription = elementType.IsClass ? "class" : "interface";

                throw new SchemaException($"A {typeDescription} based schema type must have a {nameof(SchemaTypeAttribute)} applied ({typeInfo.Name})");
            }

            return new GraphqlSchemaTypeDescriptor(elementType, GraphqlSchemaType.Scalar, elementType!.Name);
        }

        public static Directive[] GetAuthDirectivesOrDefault(this SystemType type)
        {
            var attributes = type
                .GetCustomAttributes<AuthDirectiveBaseAttribute>(true)
                .AsReadOnlyCollection();

            return attributes.GetAuthDirectivesOrDefault();
        }

        private static string GetNamespaceBasedName(string typeNamespace, string excludeNamespacePrefix, string name)
        {
            excludeNamespacePrefix ??= string.Empty;

            if (excludeNamespacePrefix.IsNullOrEmpty())
            {
                return name;
            }

            typeNamespace ??= string.Empty;

            var namePrefix = typeNamespace
                .Replace(excludeNamespacePrefix, string.Empty)
                .Replace(".", string.Empty);

            return $"{namePrefix}{name ?? string.Empty}";
        }
    }
}