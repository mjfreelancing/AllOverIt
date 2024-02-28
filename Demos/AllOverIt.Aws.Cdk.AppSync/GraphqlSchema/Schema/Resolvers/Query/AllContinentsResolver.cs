﻿namespace GraphqlSchema.Schema.Resolvers.Query
{
    internal sealed class AllContinentsResolver : HttpGetResolver
    {
        // this class is used for demonstrating registration via a factory using a common base class
        public AllContinentsResolver(string apiKey)
            : base("/continents", apiKey)
        {
        }
    }
}