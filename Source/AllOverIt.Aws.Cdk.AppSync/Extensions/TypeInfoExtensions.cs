using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Exceptions;
using AllOverIt.Extensions;
using System.Linq;
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

            return elementType.GetGraphqlTypeDescriptor();
        }

        public static GraphqlSchemaTypeDescriptor GetGraphqlTypeDescriptor(this SystemType type, ParameterInfo parameterInfo)
        {
            var elementType = type.IsArray
                ? type.GetElementType()
                : type;

            return elementType.GetGraphqlTypeDescriptor();
        }

        private static GraphqlSchemaTypeDescriptor GetGraphqlTypeDescriptor(this SystemType type)
        {
            var typeInfo = type.GetTypeInfo();
            var typeDescription = type.IsClass ? "class" : "interface";

            // SchemaTypeAttribute indicates if this is an object, scalar, interface, input type (cannot be on an array)
            var schemaTypeAttributes = typeInfo
                .GetCustomAttributes<SchemaTypeBaseAttribute>(true)
                .AsReadOnlyCollection();

            if (schemaTypeAttributes.Any())
            {
                if (schemaTypeAttributes.Count > 1)
                {
                    throw new SchemaException($"The {typeDescription} {typeInfo.Name} contains more than one schema type attribute");
                }

                var schemaTypeAttribute = schemaTypeAttributes.Single();
                return new GraphqlSchemaTypeDescriptor(type, schemaTypeAttribute.GraphqlSchemaType, schemaTypeAttribute.Name ?? typeInfo.Name);
            }

            if (type != typeof(string) && (type.IsClass || type.IsInterface))
            {
                throw new SchemaException($"A {typeDescription} based schema type must have a {nameof(SchemaTypeAttribute)} applied ({typeInfo.Name})");
            }

            return new GraphqlSchemaTypeDescriptor(type, GraphqlSchemaType.Scalar, type!.Name);
        }
    }
}