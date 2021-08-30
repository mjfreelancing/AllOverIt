using System;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Directives
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method)]
    public abstract class AuthDirectiveBaseAttribute : Attribute
    {
        public AuthDirectiveMode Mode { get; }

        protected AuthDirectiveBaseAttribute(AuthDirectiveMode mode)
        {
            Mode = mode;
        }
    }
}