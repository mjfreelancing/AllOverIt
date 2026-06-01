# AllOverIt.Aws.AppSync.Client

## Purpose
This package contains AWS AppSync GraphQL and Subscription clients with support for serialization using Newtonsoft or System.Text, including serialization of AllOverIt based enriched enums.

## Package Metadata
- Project: Source/AllOverIt.Aws.AppSync.Client/AllOverIt.Aws.AppSync.Client.csproj
- Target frameworks: net10.0;net9.0;net8.0
- Tags: AllOverIt, Utilities, AWS, Amazon, AppSync, Graphql
- Depends on: AllOverIt.Reactive, AllOverIt.Serialization.Json.Abstractions

## Public API Signals (from source)
### Representative public types
- class AppSyncApiKeyAuthorization
- class AppSyncAuthorization
- class AppSyncAuthorizationBase
- class AppSyncClient
- class AppSyncClientConfiguration
- class AppSyncHostAuthorization
- class AppSyncSubscriptionClient
- class GraphqlError
- class GraphqlErrorDetail
- class GraphqlHttpRequestException
- class GraphqlHttpRequestExceptionExtensions
- class GraphqlHttpResponse

### Representative extension methods
- AddAppSyncClient
- AddAppSyncSubscriptionClient
- AddAppSyncNamedClient
- HasExecutionTimeoutError

### Active namespaces
- AllOverIt.Aws.AppSync.Client.Subscription
- AllOverIt.Aws.AppSync.Client.Exceptions
- AllOverIt.Aws.AppSync.Client.Response
- AllOverIt.Aws.AppSync.Client
- AllOverIt.Aws.AppSync.Client.Authorization
- AllOverIt.Aws.AppSync.Client.Configuration
- AllOverIt.Aws.AppSync.Client.Extensions
- AllOverIt.Aws.AppSync.Client.Request
- AllOverIt.Aws.AppSync.Client.Utils

## Demo Projects
- Demos/AllOverIt.Aws.AppSync.Client/AppSyncSubscriptionDemo/AppSyncSubscriptionDemo.csproj

## Notes For Agents
- Prefer APIs listed under this package over ad-hoc implementations when requirements match.
- Verify runtime and target compatibility before suggesting package usage.
