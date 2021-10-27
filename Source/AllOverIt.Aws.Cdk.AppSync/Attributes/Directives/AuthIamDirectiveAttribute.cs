namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Directives
{
    /// <summary>Applies an IAM Authorization directive.</summary>
    public sealed class AuthIamDirectiveAttribute : AuthDirectiveBaseAttribute
    {
        /// <summary>Constructor.</summary>
        public AuthIamDirectiveAttribute()
            : base(AuthDirectiveMode.Iam)
        {
        }
    }
}