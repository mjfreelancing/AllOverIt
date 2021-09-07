using AllOverIt.Aws.Cdk.AppSync.Attributes.Datasources;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;
using AllOverIt.Aws.Cdk.AppSync.Schema;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;
using GraphqlSchema.Schema.Inputs;
using GraphqlSchema.Schema.Mappings.Mutation;
using GraphqlSchema.Schema.Types;

namespace GraphqlSchema.Schema
{
    internal interface IAppSyncDemoMutationDefinition : IMutationDefinition
    {
        [SchemaTypeRequired]
        [LambdaDataSource(Constants.Function.AddCountry, typeof(AddCountryMapping))]
        ICountry AddCountry([SchemaTypeRequired] ICountryInput country);

        [SchemaTypeRequired]
        [LambdaDataSource(Constants.Function.UpdateCountry, typeof(UpdateCountryMapping))]
        ICountry UpdateCountry([SchemaTypeRequired] ICountryInput country);

        // splitting out arguments so the subscription can filter
        [SchemaTypeRequired]
        [NoneDataSource(nameof(AddLanguage), typeof(AddLanguageMapping))]
        ILanguage AddLanguage(ILanguageInput language);
    }
}