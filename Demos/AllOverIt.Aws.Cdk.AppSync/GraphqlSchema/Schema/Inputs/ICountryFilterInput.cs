using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Inputs
{
    [SchemaType("CountryFilterInput", GraphqlSchemaType.Input)]
    internal interface ICountryFilterInput
    {
        public IStringQueryOperatorInput Code { get; }
        public IStringQueryOperatorInput Currency { get; }
        public IStringQueryOperatorInput Continent { get; }
    }
}