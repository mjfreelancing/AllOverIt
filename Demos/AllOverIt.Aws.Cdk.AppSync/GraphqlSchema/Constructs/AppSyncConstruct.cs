using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Aws.Cdk.AppSync.Mapping;
using Amazon.CDK.AWS.AppSync;
using Constructs;
using GraphqlSchema.Schema;
using GraphqlSchema.Schema.Mappings;
using GraphqlSchema.Schema.Mappings.Query;
using GraphqlSchema.Schema.Types;
using System;
using System.Collections.Generic;
using SystemType = System.Type;

namespace GraphqlSchema.Constructs
{
    internal sealed class AppSyncConstruct : Construct
    {
        public AppSyncConstruct(Construct scope, AppSyncDemoAppProps appProps, AuthorizationMode authMode)
            : base(scope, "AppSync")
        {
            // Providing the mapping for IAppSyncDemoQueryDefinition.CountryLanguage manually via code (see below)
            var mappingTemplates = new MappingTemplates();

            // using these just for convenience of explanation
            var noneMapping = new NoneResponseMapping();

#if false

            // This demo is using the [RequestResponseMapping] attributes to define all mappings, but they can alternatively be coded
            // using either of the two approaches below, and pass the 'mappingTemplates' to the AppSyncDemoGraphql instance, which can
            // then be passed to the base AppGraphqlBase class.

            var countriesMapping = new HttpGetResponseMapping("/countries", "ApiKey");
            var countryCodesMapping = new HttpGetResponseMapping("/countryCodes", "ApiKey");
            var continentsMapping = new HttpGetResponseMapping("/continents", "ApiKey");

            // Coded approach #1
            mappingTemplates.RegisterMappings("Query.Continents", noneMapping.RequestMapping, noneMapping.ResponseMapping);
            mappingTemplates.RegisterMappings("Query.Continents.Countries", countriesMapping.RequestMapping, countriesMapping.ResponseMapping);
            mappingTemplates.RegisterMappings("Query.Continents.CountryCodes", countryCodesMapping.RequestMapping, countryCodesMapping.ResponseMapping);
            mappingTemplates.RegisterMappings("Query.AllContinents", continentsMapping.RequestMapping, continentsMapping.ResponseMapping);

            // Coded approach #2
            mappingTemplates.RegisterQueryMappings(
                // Query.Continents
                Mapping.Template(nameof(IAppSyncDemoQueryDefinition.Continents), noneMapping.RequestMapping,noneMapping.ResponseMapping,
                    new[]
                    {
                        // Query.Continents.Countries
                        Mapping.Template(nameof(IContinent.Countries), countriesMapping.RequestMapping, countriesMapping.ResponseMapping),
                        
                        // Query.Continents.CountryCodes
                        Mapping.Template(nameof(IContinent.CountryCodes), countryCodesMapping.RequestMapping,countryCodesMapping.ResponseMapping)
                    }),

                // Query.AllContinents
                Mapping.Template(nameof(IAppSyncDemoQueryDefinition.AllContinents), continentsMapping.RequestMapping,continentsMapping.ResponseMapping)
            );

#endif

            mappingTemplates.RegisterMappings("Query.CountryLanguage", noneMapping.RequestMapping, noneMapping.ResponseMapping);

            // Registering mapping types that don't have a default constructor (so runtime arguments can be provided)
            var mappingTypeFactory = new MappingTypeFactory();
            mappingTypeFactory.Register<ContinentLanguagesMapping>(() => new ContinentLanguagesMapping(true));

            // Based on a base class type
            mappingTypeFactory.Register<HttpGetResponseMapping>(type => (IRequestResponseMapping)Activator.CreateInstance(type, "super_secret_api_key" ));

            // DateType doesn't have an attribute. Without one, it would be named "DateType", except when overriden like so:
            var typeNameOverrides = new Dictionary<SystemType, string>
            {
                { typeof(DateType), "CustomDateType" },
                { typeof(DateFormat), "CustomDateFormat" },
            };

            var graphql = new AppSyncDemoGraphql(this, appProps, authMode, typeNameOverrides, mappingTemplates, mappingTypeFactory);

            graphql
                .AddSchemaQuery<IAppSyncDemoQueryDefinition>()
                .AddSchemaMutation<IAppSyncDemoMutationDefinition>()
                .AddSchemaSubscription<IAppSyncDemoSubscriptionDefinition>();
        }
    }
}