namespace AllOverIt.Aws.Cdk.AppSync
{
    /// <summary>Indicates how a HTTP datasource determines its endpoint.</summary>
    public enum EndpointSource
    {
        /// <summary>Indicates the endpoint value is explicit.</summary>
        Explicit,

        /// <summary>Indicates the endpoint value refers to an Import key.</summary>
        ImportValue,

        /// <summary>Indicates the endpoint value refers to an environment variable name.</summary>
        EnvironmentVariable
    }
}