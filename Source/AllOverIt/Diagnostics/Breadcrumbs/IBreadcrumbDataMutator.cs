namespace AllOverIt.Diagnostics.Breadcrumbs
{
    /// <summary>Provides support for mutating the metadata and tags on a <see cref="BreadcrumbData"/> instance.</summary>
    public interface IBreadcrumbDataMutator
    {
        /// <summary>Replaces the existing metadata with the provided object.</summary>
        /// <param name="metadata">The metadata to set on the <see cref="BreadcrumbData"/> instance.</param>
        /// <returns>This same instance to support a fluent syntax.</returns>
        IBreadcrumbDataMutator WithMetadata(object metadata);

        /// <summary>Replaces the existing tags with the provided tag.</summary>
        /// <param name="tag">The tag to set on the <see cref="BreadcrumbData"/> instance.</param>
        /// <returns>This same instance to support a fluent syntax.</returns>
        IBreadcrumbDataMutator WithTag(string tag);

        /// <summary>Replaces the existing tags with the provided tags.</summary>
        /// <param name="tags">The tags to set on the <see cref="BreadcrumbData"/> instance.</param>
        /// <returns>This same instance to support a fluent syntax.</returns>
        IBreadcrumbDataMutator WithTags(params string[] tags);
    }
}
