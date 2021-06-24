using AllOverIt.Aws.Cdk.AppSync.Helpers;
using AllOverIt.Aws.Cdk.AppSync.MappingTemplates;

namespace GraphqlSchema
{
    internal sealed class SolarDigestMappingTemplates : MappingTemplatesBase
    {
        private string _defaultRequestMapping;
        private string _defaultResponseMapping;

        public override string DefaultRequestMapping
        {
            get
            {
                _defaultRequestMapping ??= StringHelpers.Prettify(
                    @"
                    {
                      ""version"" : ""2017-02-28"",
                      ""operation"": ""Invoke"",
                      ""payload"": $util.toJson($ctx.args)
                    }"
                );

                return _defaultRequestMapping;
            }
        }

        public override string DefaultResponseMapping
        {
            get
            {
                _defaultResponseMapping ??= StringHelpers.Prettify(
                    "$util.toJson($ctx.result.payload)"
                );

                return _defaultResponseMapping;
            }
        }
    }
}