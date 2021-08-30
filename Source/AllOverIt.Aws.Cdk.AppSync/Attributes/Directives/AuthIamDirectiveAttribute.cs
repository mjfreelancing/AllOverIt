namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Directives
{
    public sealed class AuthIamDirectiveAttribute : AuthDirectiveBaseAttribute
    {
        public AuthIamDirectiveAttribute()
            : base(AuthDirectiveMode.Iam)
        {
        }
    }
}