using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("CountryConnection", GraphqlSchemaType.Type)]
    internal interface ICountryConnection : IConnection<ICountryEdge, ICountry>
    {
    }
}