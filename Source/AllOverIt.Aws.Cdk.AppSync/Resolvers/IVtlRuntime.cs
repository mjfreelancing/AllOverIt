namespace AllOverIt.Aws.Cdk.AppSync.Resolvers
{
    /// <summary>Represents an AppSync resolver using a VTL request and response mapping.</summary>
    public interface IVtlRuntime : IResolverRuntime
    {
        /// <summary>The VTL request mapping as a string.</summary>
        public string RequestMapping { get; }

        /// <summary>The VTL response mapping as a string.</summary>
        public string ResponseMapping { get; }
    }
}