﻿using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("State")]
    internal interface IState : ISchemaTypeBase
    {
        [SchemaTypeRequired]
        public string Name { get; }
    }
}