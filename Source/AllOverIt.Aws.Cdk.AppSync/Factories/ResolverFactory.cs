using AllOverIt.Aws.Cdk.AppSync.Extensions;
using AllOverIt.Aws.Cdk.AppSync.MappingTemplates;
using AllOverIt.Extensions;
using AllOverIt.Helpers;
using Amazon.CDK.AWS.AppSync;
using System.Reflection;

namespace AllOverIt.Aws.Cdk.AppSync.Factories
{
    public sealed class ResolverFactory : IResolverFactory
    {
        private readonly GraphqlApi _graphQlApi;
        private readonly IMappingTemplates _mappingTemplates;
        private readonly IDataSourceFactory _dataSourceFactory;

        public ResolverFactory(GraphqlApi graphQlApi, IMappingTemplates mappingTemplates, IDataSourceFactory dataSourceFactory)
        {
            _graphQlApi = graphQlApi.WhenNotNull(nameof(graphQlApi));
            _mappingTemplates = mappingTemplates.WhenNotNull(nameof(mappingTemplates));
            _dataSourceFactory = dataSourceFactory.WhenNotNull(nameof(dataSourceFactory));
        }

        public void ConstructResolverIfRequired(string parentName, string schemaTypeName, MemberInfo methodInfo)
        {
            var dataSource = methodInfo.GetDataSource(_dataSourceFactory);           // optionally specified via a custom attribute

            if (dataSource != null)
            {
                var propertyName = methodInfo.Name;
                var mappingTemplateKey = parentName.IsNullOrEmpty() ? propertyName : $"{parentName}.{propertyName}";

                _ = new Resolver(_graphQlApi, $"{schemaTypeName}{propertyName}Resolver", new ResolverProps
                {
                    Api = _graphQlApi,
                    DataSource = dataSource,
                    TypeName = schemaTypeName,
                    FieldName = propertyName.GetGraphqlName(),
                    RequestMappingTemplate = MappingTemplate.FromString(_mappingTemplates.GetRequestMapping(mappingTemplateKey)),
                    ResponseMappingTemplate = MappingTemplate.FromString(_mappingTemplates.GetResponseMapping(mappingTemplateKey))
                });
            }
        }
    }
}