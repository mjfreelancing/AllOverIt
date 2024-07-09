namespace AllOverIt.Aws.Cdk.AppSync.Resolvers
{
    /// <summary>Represents an AppSync resolver using VTL request and response handlers.</summary>
    public interface IVtlRuntime : IResolverRuntime
    {
        /// <summary>The VTL request handler as a string.</summary>
        public string? RequestMapping { get; }

        /// <summary>The VTL response handler as a string.</summary>
        public string ResponseMapping { get; }
    }
}