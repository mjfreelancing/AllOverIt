using System;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Directives
{
    /// <summary>A base Authorization directive.</summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method)]
    public abstract class AuthDirectiveBaseAttribute : Attribute
    {
        /// <summary>The authorization mode.</summary>
        public AuthDirectiveMode Mode { get; }

        /// <summary>Constructor.</summary>
        /// <param name="mode">The authorization mode.</param>
        protected AuthDirectiveBaseAttribute(AuthDirectiveMode mode)
        {
            Mode = mode;
        }
    }
}