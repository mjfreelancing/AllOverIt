using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace GraphqlSchema.Schema.Inputs.Globe
{
    // Testing the use of namespaces => should produce 'GlobeContinentFilterInput' (the last argument can be null/empty)
    [SchemaInput("GraphqlSchema.Schema.Inputs", "ContinentFilterInput")]
    internal interface IContinentFilterInput
    {
        public IStringQueryOperatorInput Code();
    }
}