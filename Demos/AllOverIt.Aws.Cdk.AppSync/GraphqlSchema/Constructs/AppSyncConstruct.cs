// The .csproj has commented out definition for USE_CODE_FIRST_RESOLVERS.
//
// When not registering resolvers via code, the schema definition requires a resolver attribute (such as UnitResolver)
// that provides the VTL or Code based resolver type, as well as a datasourceId.
//
// When registering the VTL or Code based resolver via code, the schema still requires a resolver attribute
// but it only needs to provide the datasourceId.
//
// This demo code shows two equivalent approaches to performing the code-based registration; which may be
// required for resolvers that require arguments to be passed to their constructor.

#if USE_CODE_FIRST_RESOLVERS        // Defined in the project
//#define CODE_FIRST_APPROACH_1
#define CODE_FIRST_APPROACH_2
#endif

using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Aws.Cdk.AppSync.Resolvers;
using Amazon.CDK.AWS.AppSync;
using Constructs;
using GraphqlSchema.Schema;
using GraphqlSchema.Schema.Resolvers;
using GraphqlSchema.Schema.Resolvers.Query;
using GraphqlSchema.Schema.Types;
using System;
using System.Collections.Generic;
using SystemType = System.Type;

#if CODE_FIRST_APPROACH_2
using AllOverIt.Aws.Cdk.AppSync.Extensions;
using GraphqlSchema.Schema.Types.Globe;
using Resolver = AllOverIt.Aws.Cdk.AppSync.Resolvers.Resolver;
#endif

namespace GraphqlSchema.Constructs
{
    internal sealed class AppSyncConstruct : Construct
    {
        public AppSyncConstruct(Construct scope, AppSyncDemoAppProps appProps, AuthorizationMode authMode)
            : base(scope, "AppSync")
        {
            var resolverRegistry = new ResolverRegistry();

#if USE_CODE_FIRST_RESOLVERS
            var noneResolver = new NoneVtlResolver();

            const string apiKey = "super_secret_api_key";

            var countriesResolver = new ContinentsCountriesResolver(apiKey);
            var countryCodesResolver = new ContinentsCountryCodesResolver();
            var continentsResolver = new AllContinentsResolver(apiKey);

#if CODE_FIRST_APPROACH_1
            // Coded approach #1
            resolverRegistry.RegisterResolver("Query.Continents", noneResolver);
            resolverRegistry.RegisterResolver("Query.Continents.Countries", countriesResolver);
            resolverRegistry.RegisterResolver("Query.Continents.CountryCodes", countryCodesResolver);
            resolverRegistry.RegisterResolver("Query.AllContinents", continentsResolver);
#else
            // Coded approach #2
            resolverRegistry.RegisterQueryResolvers(
                // Query.Continents
                Resolver.Template(nameof(IAppSyncDemoQueryDefinition.Continents), noneResolver,
                    [
                        // Query.Continents.Countries
                        Resolver.Template(nameof(IContinent.Countries), countriesResolver),
                        
                        // Query.Continents.CountryCodes
                        Resolver.Template(nameof(IContinent.CountryCodes), countryCodesResolver)
                    ]),

                // Query.AllContinents
                Resolver.Template(nameof(IAppSyncDemoQueryDefinition.AllContinents), continentsResolver)
            );
#endif

#endif

            // The 'CountryLanguage' field has '[UnitResolver(Constants.NoneDataSource.CountryLanguage)]', where 
            // Constants.NoneDataSource.CountryLanguage refers to the Id of the datasource to use. The datasources
            // are registered in AppSyncDemoGraphql. This shows how a resolver can be created in advance that requires
            // additional arguments (known at the time of registration).
            resolverRegistry.RegisterResolver("Query.CountryLanguage", new CountryLanguageResolver("country_code", "country_name"));

            // Registering resolver types that don't have a default constructor (so runtime arguments can be provided).
            var resolverFactory = new ResolverFactory();
            resolverFactory.Register<ContinentLanguagesResolver>(() => new ContinentLanguagesResolver(true));

            // Register a factory method based on a base class type.
            resolverFactory.Register<HttpGetVtlResolver>(type => (IVtlRuntime) Activator.CreateInstance(type, "super_secret_api_key"));

            // DateType doesn't have an attribute. Without one, it would be named "DateType", except when overriden like so:
            var typeNameOverrides = new Dictionary<SystemType, string>
            {
                { typeof(DateType), "CustomDateType" },
                { typeof(DateFormat), "CustomDateFormat" },
            };

            var graphql = new AppSyncDemoGraphql(this, appProps, authMode, typeNameOverrides, resolverRegistry, resolverFactory);

            graphql
                .AddSchemaQuery<IAppSyncDemoQueryDefinition>()
                .AddSchemaMutation<IAppSyncDemoMutationDefinition>()
                .AddSchemaSubscription<IAppSyncDemoSubscriptionDefinition>();
        }
    }
}