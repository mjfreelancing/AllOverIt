using System;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public abstract class SchemaTypeBaseAttribute : Attribute
    {
        public GraphqlSchemaType GraphqlSchemaType { get; }
        public string Name { get; }

        protected SchemaTypeBaseAttribute(string name, GraphqlSchemaType graphqlSchemaType)
        {
            GraphqlSchemaType = graphqlSchemaType;
            Name = name;
        }
    }
}