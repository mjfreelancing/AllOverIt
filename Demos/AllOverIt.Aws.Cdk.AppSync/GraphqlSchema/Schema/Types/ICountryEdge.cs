using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("CountryEdge")]
    internal interface ICountryEdge : IEdge<ICountry>
    {
    }
}