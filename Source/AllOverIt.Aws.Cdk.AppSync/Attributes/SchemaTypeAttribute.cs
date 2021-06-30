using System;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public sealed class SchemaTypeAttribute : Attribute
    {
        public GraphqlSchemaType GraphqlSchemaType { get; }
        public string Name { get; }

        public SchemaTypeAttribute(string name, GraphqlSchemaType graphqlSchemaType = GraphqlSchemaType.Type)
        {
            GraphqlSchemaType = graphqlSchemaType;
            Name = name;
        }
    }
}