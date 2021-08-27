﻿using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Aws.Cdk.AppSync.Mapping;
using Amazon.CDK;
using Amazon.CDK.AWS.AppSync;

namespace GraphqlSchema
{
    internal sealed class AppSyncDemoGraphql : AppGraphqlBase
    {
        public AppSyncDemoGraphql(Construct scope, AppSyncDemoAppProps appProps, IAuthorizationMode authMode, MappingTemplates mappingTemplates,
            MappingTypeFactory mappingTypeFactory)
            : base(scope, "GraphQl", GetGraphqlApiProps(appProps, authMode), mappingTemplates, mappingTypeFactory)
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