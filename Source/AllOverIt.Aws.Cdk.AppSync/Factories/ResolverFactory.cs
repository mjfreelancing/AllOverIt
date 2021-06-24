using AllOverIt.Aws.Cdk.AppSync.Extensions;
using AllOverIt.Aws.Cdk.AppSync.MappingTemplates;
using AllOverIt.Helpers;
using Amazon.CDK.AWS.AppSync;
using System.Reflection;
using SystemType = System.Type;

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

        public void ConstructResolverIfRequired(SystemType type, MemberInfo methodInfo)
        {
            var dataSource = methodInfo.GetDataSource(_dataSourceFactory);           // optionally specified via a custom attribute

            if (dataSource != null)
            {
                var propertyName = methodInfo.Name;

                var mappingTemplateKey = methodInfo.GetFunctionName();

                _ = new Resolver(_graphQlApi, $"{type.Name}{propertyName}Resolver", new ResolverProps
                {
                    Api = _graphQlApi,
                    DataSource = dataSource,
                    TypeName = type.Name,
                    FieldName = propertyName.GetGraphqlName(),
                    RequestMappingTemplate = MappingTemplate.FromString(_mappingTemplates.GetRequestMapping(mappingTemplateKey)),
                    ResponseMappingTemplate = MappingTemplate.FromString(_mappingTemplates.GetResponseMapping(mappingTemplateKey))
                });
            }
        }
    }
}