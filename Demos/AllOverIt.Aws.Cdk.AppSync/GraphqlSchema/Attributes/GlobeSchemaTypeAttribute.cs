using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace GraphqlSchema.Attributes
{
    public sealed class GlobeSchemaTypeAttribute : SchemaTypeBaseAttribute
    {
        public GlobeSchemaTypeAttribute(string name)
            : base("GraphqlSchema.Schema.Types", name, GraphqlSchemaType.Type)
        {
        }
    }
}