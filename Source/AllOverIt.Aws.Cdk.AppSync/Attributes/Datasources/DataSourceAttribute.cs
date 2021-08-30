﻿using System;
using System.Text.RegularExpressions;
using AllOverIt.Aws.Cdk.AppSync.Mapping;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Datasources
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class DataSourceAttribute : Attribute
    {
        // used for lookup in the DataSourceFactory
        public abstract string LookupKey { get; }
        public Type MappingType { get; }
        public string Description { get; }

        protected static string SanitiseLookupKey(string lookupKey)
        {
            // exclude everything exception alphanumeric and dashes
            return Regex.Replace(lookupKey, @"[^\w]", "", RegexOptions.None);
        }

        protected DataSourceAttribute(Type mappingType, string description)
        {
            // Will be null if the mapping is being provided via code in a MappingTemplates
            if (mappingType != null)
            {
                if (!typeof(IRequestResponseMapping).IsAssignableFrom(mappingType))
                {
                    throw new InvalidOperationException($"The type '{mappingType.FullName}' must implement '{nameof(IRequestResponseMapping)}'");
                }

                MappingType = mappingType;
            }

            Description = description;
        }
    }
}