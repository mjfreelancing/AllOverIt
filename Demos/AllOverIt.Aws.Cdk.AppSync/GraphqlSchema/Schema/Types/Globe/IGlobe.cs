using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace GraphqlSchema.Schema.Types.Globe
{
    // Testing the use of namespaces => should produce 'Globe'
    [SchemaType("GraphqlSchema.Schema.Types", null)]
    internal interface IGlobe : ISchemaTypeBase
    {
        public string Name();
    }
}