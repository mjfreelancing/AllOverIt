namespace AllOverIt.Aws.Cdk.AppSync.Mapping
{
    /// <summary>Represents an AppSync datasource request and response mapping.</summary>
    public interface IRequestResponseMapping
    {
        /// <summary>The request mapping as a string.</summary>
        public string RequestMapping { get; }

        /// <summary>The response mapping as a string.</summary>
        public string ResponseMapping { get; }

        /// <summary>The code for request and response mapping.</summary>
        public string Code { get; }
    }
}