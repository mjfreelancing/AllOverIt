using Amazon.CDK;
using Amazon.CDK.AWS.AppSync;
using GraphqlSchema.Constructs;
using System;
using CDKEnvironment = Amazon.CDK.Environment;
using SystemEnvironment = System.Environment;

namespace GraphqlSchema
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new App();

            var stack = new Stack(app, $"{Constants.AppName}V{Constants.ServiceVersion}", new StackProps
            {
                Env = new CDKEnvironment
                {
                    Account = SystemEnvironment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
                    Region = SystemEnvironment.GetEnvironmentVariable("CDK_DEFAULT_REGION")
                }
            });

            // required to test HttpDataSourceAttribute
            stack.ExportValue(Token.AsString(Constants.HttpDataSource.GetLanguageUrlExplicit), new ExportValueOptions
            {
                Name = Constants.Import.GetCountriesUrlImportName
            });

            var appProps = new AppSyncDemoAppProps
            {
                StackName = $"{Constants.AppName}Stack",
                AppName = Constants.AppName,
                Version = Constants.ServiceVersion
            };

            var authMode = new AuthorizationMode
            {
                AuthorizationType = AuthorizationType.API_KEY,
                ApiKeyConfig = new ApiKeyConfig
                {
                    Expires = Expiration.AtDate(DateTime.Now.AddDays(365))
                }
            };

            var mappingTemplates = new AppSyncDemoMappingTemplates();

            _ = new AppSyncConstruct(stack, appProps, authMode, mappingTemplates);

            _ = app.Synth();
        }
    }
}
