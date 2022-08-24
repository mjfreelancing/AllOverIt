using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;

namespace GraphqlSchema.Schema.Types
{
    public interface ISchemaTypeBase
    {
        [SchemaTypeRequired]        // leave this on the base type - good for testing subscriptions
        GraphqlTypeId Code();
    }
}