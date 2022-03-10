using System;
using System.Collections.Generic;
using System.Reflection;
using AllOverIt.Reflection;

namespace AllOverIt.Mapping
{
    /// <summary>Provides options that control how source properties are copied onto a target instance.</summary>
    public class ObjectMapperOptions
    {
        internal sealed class TargetOptions
        {
            public bool Excluded { get; set; }
            public string Alias { get; set; }
            public Func<object, object> Converter { get; set; }
        }

        // Source property to target options - updated via extension methods
        internal readonly IDictionary<string, TargetOptions> SourceTargetOptions = new Dictionary<string, TargetOptions>();

        /// <summary>The binding options used to determine how properties on the source object are discovered.</summary>
        public BindingOptions Binding { get; set; } = BindingOptions.Default;

        /// <summary>Use to filter out source properties discovered based on the <see cref="Binding"/> option used.</summary>
        public Func<PropertyInfo, bool> Filter { get; set; }
    }
}