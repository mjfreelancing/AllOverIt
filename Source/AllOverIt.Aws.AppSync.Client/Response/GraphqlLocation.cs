namespace AllOverIt.Aws.AppSync.Client.Response
{
    /// <summary>Contains location information within a query or mutation request.</summary>
    public sealed record GraphqlLocation
    {
        /// <summary>The line within the request that caused an error.</summary>
        public int Line { get; set; }

        /// <summary>The column within the indicated line that caused an error.</summary>
        public int Column { get; set; }
    }
}