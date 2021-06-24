using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Exceptions;
using Amazon.CDK.AWS.AppSync;
using System;
using System.Reflection;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync.Extensions
{
    internal static class TypeExtensions
    {
        public static GraphqlSchemaTypeDescriptor GetGraphqlTypeDescriptor(this SystemType type, MemberInfo memberInfo)
        {
            var elementType = type.IsArray
                ? type.GetElementType()
                : type;

            if (elementType == typeof(DateTime))
            {
                return GetDateTimeSchemaTypeDescriptor(elementType, memberInfo);
            }

            return elementType.GetGraphqlTypeDescriptor();
        }

        public static GraphqlSchemaTypeDescriptor GetGraphqlTypeDescriptor(this SystemType type, ParameterInfo parameterInfo)
        {
            var elementType = type.IsArray
                ? type.GetElementType()
                : type;

            if (elementType == typeof(DateTime))
            {
                return GetDateTimeSchemaTypeDescriptor(elementType, parameterInfo);
            }

            return elementType.GetGraphqlTypeDescriptor();
        }

        private static GraphqlSchemaTypeDescriptor GetGraphqlTypeDescriptor(this SystemType type)
        {
            var typeInfo = type.GetTypeInfo();

            // SchemaTypeAttribute indicates if this is an object, interface, input type (cannot be on an array)
            var schemaTypeAttribute = typeInfo.GetCustomAttribute<SchemaTypeAttribute>(true);

            if (schemaTypeAttribute != null)
            {
                return new GraphqlSchemaTypeDescriptor(type, schemaTypeAttribute!.GraphqlSchemaType, schemaTypeAttribute.Name ?? typeInfo.Name);
            }

            if (type != typeof(string) && (type.IsClass || type.IsInterface))
            {
                throw new SchemaException($"A class or interface based schema type must have a {nameof(SchemaTypeAttribute)} applied ({typeInfo.Name})");
            }

            return new GraphqlSchemaTypeDescriptor(type, GraphqlSchemaType.Scalar, type!.Name);
        }

        private static GraphqlSchemaTypeDescriptor GetDateTimeSchemaTypeDescriptor(SystemType type, ICustomAttributeProvider attributeProvider)
        {
            GraphqlSchemaTypeDescriptor CreateDescriptor(string name)
            {
                return new(type, GraphqlSchemaType.AWSScalar, name);
            }

            bool HasAttribute<TAttribute>() where TAttribute : Attribute
            {
                if (attributeProvider is MemberInfo memberInfo)
                {
                    return memberInfo.GetCustomAttribute<TAttribute>(true) != null;
                }

                if (attributeProvider is ParameterInfo parameterInfo)
                {
                    return parameterInfo.GetCustomAttribute<TAttribute>(true) != null;
                }

                throw new InvalidOperationException("Expected a MemberInfo or ParameterInfo type");
            }

            if (HasAttribute<SchemaDateTypeAttribute>())
            {
                return CreateDescriptor(nameof(GraphqlType.AwsDate));
            }
            
            if (HasAttribute<SchemaTimeTypeAttribute>())
            {
                return CreateDescriptor(nameof(GraphqlType.AwsTime));
            }
            
            return CreateDescriptor(nameof(GraphqlType.AwsDateTime));
        }
    }
}