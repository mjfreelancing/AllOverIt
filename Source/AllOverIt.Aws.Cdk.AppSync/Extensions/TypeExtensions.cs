using AllOverIt.Aws.Cdk.AppSync.Attributes.Directives;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;
using AllOverIt.Aws.Cdk.AppSync.Exceptions;
using AllOverIt.Extensions;
using Amazon.CDK.AWS.AppSync;
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

        public static GraphqlSchemaTypeDescriptor GetGraphqlTypeDescriptor(this SystemType type)
        {
            var elementType = type.GetElementTypeIfArray();
            var typeInfo = elementType!.GetTypeInfo();

            // Is the type an enum
            if (type.IsEnum)
            {
                var attribute = type.GetCustomAttribute<SchemaEnumAttribute>();

                return attribute == null
                    ? new GraphqlSchemaTypeDescriptor(elementType, GraphqlSchemaType.Enum, type.Name)
                    : new GraphqlSchemaTypeDescriptor(elementType, GraphqlSchemaType.Enum, attribute.Name);
            }

            // SchemaTypeAttribute indicates if this is a scalar, interface, or input type (cannot be on an array)
            var schemaTypeAttributes = typeInfo
                .GetCustomAttributes<SchemaTypeBaseAttribute>(true)
                .AsReadOnlyCollection();

            if (schemaTypeAttributes.Any())
            {
                if (schemaTypeAttributes.Count > 1)
                {
                    throw new SchemaException($"'{typeInfo.Name}' contains more than one schema type attribute");
                }

                var schemaTypeAttribute = schemaTypeAttributes.Single();
                return new GraphqlSchemaTypeDescriptor(elementType, schemaTypeAttribute.GraphqlSchemaType, schemaTypeAttribute.Name ?? typeInfo.Name);
            }

            // not expecting class types to be used, but check anyway
            if (elementType != typeof(string) && (elementType.IsClass || elementType.IsInterface))
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
    }
}