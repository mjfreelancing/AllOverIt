using AllOverIt.Aws.Cdk.AppSync.Attributes.Directives;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Resolvers;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;
using AllOverIt.Aws.Cdk.AppSync.Schema;
using GraphqlSchema.Schema.Inputs;
using GraphqlSchema.Schema.Inputs.Globe;
using GraphqlSchema.Schema.Resolvers.Mutation;
using GraphqlSchema.Schema.Types;

namespace GraphqlSchema.Schema
{
    internal interface IAppSyncDemoMutationDefinition : IMutationDefinition
    {
        [UnitResolver(typeof(SendEmailResolver), Constants.EventBridgeDataSource.Default)]
        bool SendEmail(string email);

        [SchemaTypeRequired]
        [UnitResolver(typeof(AddCountryResolver), Constants.LambdaDataSource.AddCountry)]
        ICountry AddCountry([SchemaTypeRequired] ICountryInput country);

#if DEBUG   // Using RELEASE mode to deploy without these (DEBUG mode is used to check Synth output)
        [AuthLambdaDirective]
#endif
        [SchemaTypeRequired]
        [UnitResolver(typeof(UpdateCountryResolver), Constants.LambdaDataSource.UpdateCountry)]
        ICountry UpdateCountry([SchemaTypeRequired] ICountryInput country);

        [SchemaTypeRequired]
        [UnitResolver(typeof(AddLanguageResolver), Constants.NoneDataSource.Mutation)]
        ILanguage AddLanguage([SchemaTypeRequired] ILanguageInput language);
    }
}