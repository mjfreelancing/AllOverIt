namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Directives
{
    /// <summary>Applies a Lambda Authorization directive.</summary>
    public sealed class AuthLambdaDirectiveAttribute : AuthDirectiveBaseAttribute
    {
        /// <summary>Constructor.</summary>
        public AuthLambdaDirectiveAttribute()
            : base(AuthDirectiveMode.Lambda)
        {
        }
    }
}