using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Inputs
{
    [SchemaInput("CountryFilterInput")]
    internal interface ICountryFilterInput
    {
        public IStringQueryOperatorInput Code { get; }
        public IStringQueryOperatorInput Currency { get; }
        public IStringQueryOperatorInput Continent { get; }
    }
}