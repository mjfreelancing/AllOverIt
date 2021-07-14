using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("CountryConnection")]
    internal interface ICountryConnection : IConnection<ICountryEdge, ICountry>
    {
    }
}