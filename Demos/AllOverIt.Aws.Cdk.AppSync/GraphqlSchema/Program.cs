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
        static void Main()
        {
            var app = new App();

            var stack1 = new Stack(app, $"{Constants.AppName}V{Constants.ServiceVersion}Stack1", new StackProps
            {
                Env = new CDKEnvironment
                {
                    Account = SystemEnvironment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
                    Region = SystemEnvironment.GetEnvironmentVariable("CDK_DEFAULT_REGION")
                }
            });

            // required to test HttpDataSourceAttribute
            stack1.ExportValue(Token.AsString(Constants.HttpDataSource.GetLanguageUrlExplicit), new ExportValueOptions
            {
                Name = Constants.Import.GetCountriesUrlImportName
            });

            var stack2 = new Stack(app, $"{Constants.AppName}V{Constants.ServiceVersion}Stack2", new StackProps
            {
                Env = new CDKEnvironment
                {
                    Account = SystemEnvironment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
                    Region = SystemEnvironment.GetEnvironmentVariable("CDK_DEFAULT_REGION")
                }
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

            _ = new AppSyncConstruct(stack2, appProps, authMode);

            _ = app.Synth();
        }
    }
}
