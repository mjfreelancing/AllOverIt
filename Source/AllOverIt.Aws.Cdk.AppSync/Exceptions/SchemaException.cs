using System;

namespace AllOverIt.Aws.Cdk.AppSync.Exceptions
{
    public sealed class SchemaException : Exception
    {
        public SchemaException(string message)
            : base(message)
        {
        }
    }
}