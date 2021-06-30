using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("State", GraphqlSchemaType.Type)]
    internal interface IState
    {
        [SchemaTypeRequired]
        public string Code { get; }

        [SchemaTypeRequired]
        public string Name { get; }
    }
}