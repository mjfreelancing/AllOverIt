using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace GraphqlSchema.Schema.Inputs.Globe
{
    // Testing the use of namespaces => should produce 'GlobeCountryFilterInput' (the last argument can be null/empty)
    [SchemaInput("GraphqlSchema.Schema.Inputs", "CountryFilterInput")]
    internal interface ICountryFilterInput
    {
        IStringQueryOperatorInput Code();
        IStringQueryOperatorInput Currency();
        IStringQueryOperatorInput Continent();
    }
}