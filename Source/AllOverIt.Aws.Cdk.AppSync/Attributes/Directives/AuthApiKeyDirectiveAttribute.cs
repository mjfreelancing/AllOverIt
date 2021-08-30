namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Directives
{
    public sealed class AuthApiKeyDirectiveAttribute : AuthDirectiveBaseAttribute
    {
        public AuthApiKeyDirectiveAttribute()
            : base(AuthDirectiveMode.ApiKey)
        {
        }
    }
}