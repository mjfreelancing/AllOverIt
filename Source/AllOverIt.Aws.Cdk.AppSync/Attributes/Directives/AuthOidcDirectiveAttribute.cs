namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Directives
{
    /// <summary>Applies an OIDC Authorization directive.</summary>
    public sealed class AuthOidcDirectiveAttribute : AuthDirectiveBaseAttribute
    {
        /// <summary>Constructor.</summary>
        public AuthOidcDirectiveAttribute()
            : base(AuthDirectiveMode.Oidc)
        {
        }
    }
}