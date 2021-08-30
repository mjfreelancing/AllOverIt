using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Schema;
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
    }
}