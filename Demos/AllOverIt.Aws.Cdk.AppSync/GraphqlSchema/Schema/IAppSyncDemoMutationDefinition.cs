using AllOverIt.Aws.Cdk.AppSync.Attributes.DataSources;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Directives;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;
using AllOverIt.Aws.Cdk.AppSync.Schema;
using GraphqlSchema.Schema.Inputs;
using GraphqlSchema.Schema.Inputs.Globe;
using GraphqlSchema.Schema.Mappings.Mutation;
using GraphqlSchema.Schema.Types;

namespace GraphqlSchema.Schema
{
    internal interface IAppSyncDemoMutationDefinition : IMutationDefinition
    {
        [SchemaTypeRequired]
        [LambdaDataSource(Constants.Function.AddCountry, typeof(AddCountryMapping))]
        ICountry AddCountry([SchemaTypeRequired] ICountryInput country);

        [AuthLambdaDirective]
        [SchemaTypeRequired]
        [LambdaDataSource(Constants.Function.UpdateCountry, typeof(UpdateCountryMapping))]
        ICountry UpdateCountry([SchemaTypeRequired] ICountryInput country);

        [SchemaTypeRequired]
        [NoneDataSource(nameof(AddLanguage), typeof(AddLanguageMapping))]
        ILanguage AddLanguage([SchemaTypeRequired] ILanguageInput language);
    }
}