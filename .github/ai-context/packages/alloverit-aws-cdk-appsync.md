# AllOverIt.Aws.Cdk.AppSync

## Purpose
This package greatly simplifies the generation of AWS Graphql schemas, including their associated resolver handler configuration, using a code-first approach.

## Package Metadata
- Project: Source/AllOverIt.Aws.Cdk.AppSync/AllOverIt.Aws.Cdk.AppSync.csproj
- Target frameworks: net10.0;net9.0;net8.0
- Tags: AllOverIt, Utilities, AWS, Amazon, AppSync, CDK, Graphql
- Depends on: AllOverIt

## Public API Signals (from source)
### Representative public types
- interface IConnection
- class AppGraphqlBase
- class AppGraphqlProps
- class AuthApiKeyDirectiveAttribute
- class AuthCognitoDirectiveAttribute
- class AuthDirectiveBaseAttribute
- class AuthIamDirectiveAttribute
- class AuthLambdaDirectiveAttribute
- class AuthOidcDirectiveAttribute
- class AwsTypeDate
- class AwsTypeDateTime
- class AwsTypeEmail

### Representative extension methods
- RegisterMutationResolvers
- RegisterQueryResolvers
- RegisterSubscriptionResolvers

### Active namespaces
- AllOverIt.Aws.Cdk.AppSync.Schema.Types
- AllOverIt.Aws.Cdk.AppSync
- AllOverIt.Aws.Cdk.AppSync.Attributes.Types
- AllOverIt.Aws.Cdk.AppSync.Extensions
- AllOverIt.Aws.Cdk.AppSync.Attributes.Directives
- AllOverIt.Aws.Cdk.AppSync.Resolvers
- AllOverIt.Aws.Cdk.AppSync.DataSources
- AllOverIt.Aws.Cdk.AppSync.Schema
- AllOverIt.Aws.Cdk.AppSync.Attributes.Resolvers
- AllOverIt.Aws.Cdk.AppSync.Factories

## Demo Projects
- Demos/AllOverIt.Aws.Cdk.AppSync/GraphqlSchema/GraphqlSchema.csproj

## Notes For Agents
- Prefer APIs listed under this package over ad-hoc implementations when requirements match.
- Verify runtime and target compatibility before suggesting package usage.
