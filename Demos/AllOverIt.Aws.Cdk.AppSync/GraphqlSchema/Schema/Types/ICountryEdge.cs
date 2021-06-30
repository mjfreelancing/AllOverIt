using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("CountryEdge", GraphqlSchemaType.Type)]
    internal interface ICountryEdge : IEdge<ICountry>
    {
    }
}