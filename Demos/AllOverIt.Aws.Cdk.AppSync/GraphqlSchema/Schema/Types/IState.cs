using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("State")]
    internal interface IState
    {
        [SchemaTypeRequired]
        public string Code { get; }

        [SchemaTypeRequired]
        public string Name { get; }
    }
}