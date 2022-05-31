﻿using AllOverIt.Aws.Cdk.AppSync.Attributes.Directives;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;
using GraphqlSchema.Schema.Types.Globe;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("Country")]
    internal interface ICountry : ISchemaTypeBase
    {
        public string Name();
        public string Currency();

        [AuthLambdaDirective] 
        public ILanguage[] Languages();

        public IContinent Continent();      // this is a circular reference
    }
}