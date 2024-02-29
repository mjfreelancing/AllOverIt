using AllOverIt.Aws.Cdk.AppSync.Attributes.Directives;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Resolvers;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;
using GraphqlSchema.Schema.Resolvers.Query;
using GraphqlSchema.Schema.Types.Globe;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("Country")]
    internal interface ICountry : ISchemaTypeBase
    {
        string Name();
        string Currency();

#if DEBUG   // Using RELEASE mode to deploy without these (DEBUG mode is used to check Synth output)
        [AuthLambdaDirective]
#endif
        ILanguage[] Languages();

        DateFormat DefaultDateFormat();
        DateFormat[] DateFormats();

        DateType DefaultDateType();
        DateType[] DateTypes();

        IContinent Continent();      // this is a circular reference

        [UnitResolver(typeof(PopulationResolver), Constants.HttpDataSource.GetPopulationUrlExplicit)]
        int Population();
    }
}