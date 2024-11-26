using AllOverIt.Assertion;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync
{
    internal sealed class GraphqlSchemaTypeDescriptor
    {
        public SystemType Type { get; }
        public GraphqlSchemaType SchemaType { get; }
        public string Name { get; }

        public GraphqlSchemaTypeDescriptor(SystemType type, GraphqlSchemaType schemaType, string name)
        {
            Type = type.WhenNotNull();
            SchemaType = schemaType;
            Name = name.WhenNotNullOrEmpty();
        }
    }
}