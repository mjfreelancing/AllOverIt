namespace AllOverIt.Aws.Cdk.AppSync
{
    /// <summary>Provides available authorization directive modes.</summary>
    public enum AuthDirectiveMode
    {
        /// <summary>OIDC authorization mode.</summary>
        Oidc,

        /// <summary>API KEY authorization mode.</summary>
        ApiKey,

        /// <summary>Cognito authorization mode.</summary>
        Cognito,

        /// <summary>IAM authorization mode.</summary>
        Iam
    }
}