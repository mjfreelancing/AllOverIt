using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Inputs
{
    [SchemaInput("CountryFilterInput")]
    internal interface ICountryFilterInput
    {
        public IStringQueryOperatorInput Code();
        public IStringQueryOperatorInput Currency();
        public IStringQueryOperatorInput Continent();
    }
}