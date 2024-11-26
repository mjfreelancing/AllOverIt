﻿namespace GraphqlSchema.Schema.Resolvers.Query
{
    internal sealed class ContinentsCountriesResolver : HttpGetVtlResolver
    {
        // this class is used for demonstrating registration via a factory using a common base class
        public ContinentsCountriesResolver(string apiKey)
            : base("/countries", apiKey)
        {
        }
    }
}