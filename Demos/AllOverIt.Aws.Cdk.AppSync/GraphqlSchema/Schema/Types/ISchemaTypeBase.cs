using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;

namespace GraphqlSchema.Schema.Types
{
    public interface ISchemaTypeBase
    {
        [SchemaTypeRequired]
        public GraphqlTypeId Code();
    }
}