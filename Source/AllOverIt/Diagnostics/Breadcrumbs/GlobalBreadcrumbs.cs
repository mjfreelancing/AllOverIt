using AllOverIt.Assertion;

namespace AllOverIt.Diagnostics.Breadcrumbs
{
    /// <summary>A helper to create a static <see cref="Diagnostics.Breadcrumbs.Breadcrumbs"/> for when you need a globally accessible instance
    /// for troubleshooting.</summary>
    public static class GlobalBreadcrumbs
    {
        private static IBreadcrumbs? Breadcrumbs;

        /// <summary>Gets the current global instance, creating a default instance if required.</summary>
        public static IBreadcrumbs Instance
        {
            get
            {
                Breadcrumbs ??= Create();
                return Breadcrumbs;
            }
        }

        /// <summary>Creates a new <see cref="Diagnostics.Breadcrumbs.Breadcrumbs"/> instance with a default constructed <see cref="BreadcrumbsOptions"/>.
        /// If the global instance already exists it will be replaced.</summary>
        /// <returns>A new <see cref="Diagnostics.Breadcrumbs.Breadcrumbs"/> instance.</returns>
        public static IBreadcrumbs Create()
        {
            Breadcrumbs = new Breadcrumbs(new BreadcrumbsOptions());

            return Breadcrumbs;
        }

        /// <summary>Creates a new <see cref="Diagnostics.Breadcrumbs.Breadcrumbs"/> instance with the provided options. If the global instance
        /// already exists it will be replaced.</summary>
        /// <param name="options">The breadcrumbs options to use.</param>
        /// <returns>A new <see cref="Diagnostics.Breadcrumbs.Breadcrumbs"/> instance.</returns>
        public static IBreadcrumbs Create(BreadcrumbsOptions options)
        {
            _ = options.WhenNotNull();

            // Re-create the breadcrumbs if called more than once
            Breadcrumbs = new Breadcrumbs(options);

            return Breadcrumbs;
        }

        /// <summary>Clears all breadcrumb data and releases the global <see cref="Diagnostics.Breadcrumbs.Breadcrumbs"/> instance.</summary>
        public static void Destroy()
        {
            Breadcrumbs?.Clear();
            Breadcrumbs = null;
        }
    }
}
