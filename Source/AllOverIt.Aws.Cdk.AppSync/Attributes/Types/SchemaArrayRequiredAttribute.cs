﻿using System;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Types
{
    /// <summary>Apply to schema 'type' and 'input' array types to indicate they are required.</summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public sealed class SchemaArrayRequiredAttribute : Attribute
    {
    }
}