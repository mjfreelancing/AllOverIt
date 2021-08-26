﻿using AllOverIt.Aws.Cdk.AppSync;
using Amazon.CDK;
using Amazon.CDK.AWS.AppSync;

namespace GraphqlSchema
{
    internal sealed class AppSyncDemoGraphql : AppGraphqlBase
    {
        public AppSyncDemoGraphql(Construct scope, AppSyncDemoAppProps appProps, IAuthorizationMode authMode)
            : base(scope, "GraphQl", GetGraphqlApiProps(appProps, authMode))
        {
        }

        private static GraphqlApiProps GetGraphqlApiProps(AppSyncDemoAppProps appProps, IAuthorizationMode authMode)
        {
            return new GraphqlApiProps
            {
                Name = $"{appProps.AppName} V{appProps.Version}",
                AuthorizationConfig = new AuthorizationConfig { DefaultAuthorization = authMode }
            };
        }
    }
}