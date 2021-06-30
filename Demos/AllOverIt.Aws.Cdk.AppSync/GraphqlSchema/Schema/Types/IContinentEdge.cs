﻿using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("ContinentEdge", GraphqlSchemaType.Type)]
    internal interface IContinentEdge : IEdge<IContinent>
    {
    }
}