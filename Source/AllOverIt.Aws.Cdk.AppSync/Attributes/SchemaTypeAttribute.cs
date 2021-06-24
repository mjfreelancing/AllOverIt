using System;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public sealed class SchemaTypeAttribute : Attribute
    {
        public GraphqlSchemaType GraphqlSchemaType { get; }
        public string Name { get; }

        public SchemaTypeAttribute(GraphqlSchemaType graphqlSchemaType = GraphqlSchemaType.Type, string name = default)
        {
            GraphqlSchemaType = graphqlSchemaType;
            Name = name;
        }
    }
}