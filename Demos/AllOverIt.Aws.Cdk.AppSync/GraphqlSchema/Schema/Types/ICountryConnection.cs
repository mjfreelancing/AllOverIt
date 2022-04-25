using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("CountryConnection")]
    internal interface ICountryConnection : IConnection<ICountryEdge, ICountry>
    {
    }
}