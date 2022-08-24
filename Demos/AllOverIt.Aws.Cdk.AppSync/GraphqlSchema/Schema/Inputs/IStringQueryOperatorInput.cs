using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace GraphqlSchema.Schema.Inputs
{
    [SchemaInput("StringQueryOperatorInput")]
    internal interface IStringQueryOperatorInput
    {
        string Eq();
        string Ne();
        string Gt();
        string Gte();
        string Lt();
        string Lte();
        string In();
        string NotIn();
        string Regex();
    }
}