using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;

namespace GraphqlSchema.Schema.Types
{
    public interface ISchemaTypeBase
    {
        [SchemaTypeRequired]
        public GraphqlTypeId Code();
    }
}