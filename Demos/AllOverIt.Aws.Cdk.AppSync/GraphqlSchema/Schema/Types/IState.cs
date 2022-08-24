using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("State")]
    internal interface IState : ISchemaTypeBase
    {
        string Name();
    }
}