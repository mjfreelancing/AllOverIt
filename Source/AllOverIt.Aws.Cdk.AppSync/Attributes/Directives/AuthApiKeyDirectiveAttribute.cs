namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Directives
{
    /// <summary>Applies an API KEY Authorization directive.</summary>
    public sealed class AuthApiKeyDirectiveAttribute : AuthDirectiveBaseAttribute
    {
        /// <summary>Constructor.</summary>
        public AuthApiKeyDirectiveAttribute()
            : base(AuthDirectiveMode.ApiKey)
        {
        }
    }
}