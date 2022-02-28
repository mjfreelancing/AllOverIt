using AllOverIt.Reflection;

namespace AllOverIt.Mapping
{
    /// <summary>Provides options that control how source properties are copied onto a target instance.</summary>
    public sealed class ObjectMapperOptions
    {
        /// <summary>The binding options used to determine how properties on the source object are discovered.</summary>
        public BindingOptions Binding { get; set; } = BindingOptions.Default;
    }
}