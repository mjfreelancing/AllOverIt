using AllOverIt.Assertion;

namespace AllOverIt.Diagnostics.Breadcrumbs.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="BreadcrumbData"/>.</summary>
    public static class BreadcrumbDataExtensions
    {
        /// <summary>Gets an interface that provides support for modifying metadata and tags on a <see cref="BreadcrumbData"/> instance.
        /// This approach is used in preference to making the properties public setters so it is an obvious explicit action.</summary>
        /// <param name="breadcrumbData">The <see cref="BreadcrumbData"/> to be wrapped.</param>
        /// <returns>A wrapper that provides the ability to mutate the breadcrumb data's metadata and tags.</returns>
        public static IBreadcrumbDataMutator AsMutable(this BreadcrumbData breadcrumbData)
        {
            _ = breadcrumbData.WhenNotNull();

            return new BreadcrumbDataMutator(breadcrumbData);
        }
    }
}
