using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Schema;
using GraphqlSchema.Schema.Inputs;
using GraphqlSchema.Schema.Types;

namespace GraphqlSchema.Schema
{
    internal interface IAppSyncDemoMutationDefinition : IMutationDefinition
    {
        [SchemaTypeRequired]
        [LambdaDataSource(Constants.AppName, Constants.Function.AddCountry)]
        ICountry AddCountry([SchemaTypeRequired] ICountryInput country);

        [SchemaTypeRequired]
        [LambdaDataSource(Constants.AppName, Constants.Function.UpdateCountry)]
        ICountry UpdateCountry([SchemaTypeRequired] ICountryInput country);
    }
}