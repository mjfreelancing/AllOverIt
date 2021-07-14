﻿using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("Continent")]
    internal interface IContinent
    {
        [SchemaTypeRequired]
        public GraphqlTypeId Code { get; }

        [SchemaTypeRequired]
        public string Name { get; }

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        public ICountry[] Countries { get; }
    }
}