namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Directives
{
    public sealed class AuthOidcDirectiveAttribute : AuthDirectiveBaseAttribute
    {
        public AuthOidcDirectiveAttribute()
            : base(AuthDirectiveMode.Oidc)
        {
        }
    }
}