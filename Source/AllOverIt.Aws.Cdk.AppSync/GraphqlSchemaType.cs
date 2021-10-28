namespace AllOverIt.Aws.Cdk.AppSync
{
    /// <summary>Identifies a schema type.</summary>
    public enum GraphqlSchemaType
    {
        /// <summary>Specifies a schema 'Input' ype.</summary>
        Input,

        /// <summary>Specifies a schema 'Type' ype.</summary>
        Type,

        //Interface,                pending support
        //Union,                    pending support

        /// <summary>Specifies a schema scalar ype.</summary>
        Scalar,

        /// <summary>Specifies a schema custom AWS scalar type.</summary>
        AWSScalar
    }
}