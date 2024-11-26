using AllOverIt.Assertion;

namespace AllOverIt.Diagnostics.Breadcrumbs
{
    /// <summary>A helper to create a static <see cref="Breadcrumbs"/> for when you need a globally accessible instance
    /// for troubleshooting.</summary>
    public static class GlobalBreadcrumbs
    {
        private static IBreadcrumbs? _breadcrumbs;

        /// <summary>Gets the current global instance, creating a default instance if required.</summary>
        public static IBreadcrumbs Instance
        {
            get
            {
                _breadcrumbs ??= Create();
                return _breadcrumbs;
            }
        }

        /// <summary>Creates a new <see cref="Breadcrumbs"/> instance with a default constructed <see cref="BreadcrumbsOptions"/>.
        /// If the global instance already exists it will be replaced.</summary>
        /// <returns>A new <see cref="Breadcrumbs"/> instance.</returns>
        public static IBreadcrumbs Create()
        {
            _breadcrumbs = new Breadcrumbs(new BreadcrumbsOptions());

            return _breadcrumbs;
        }

        /// <summary>Creates a new <see cref="Breadcrumbs"/> instance with the provided options. If the global instance
        /// already exists it will be replaced.</summary>
        /// <param name="options">The breadcrumbs options to use.</param>
        /// <returns>A new <see cref="Breadcrumbs"/> instance.</returns>
        public static IBreadcrumbs Create(BreadcrumbsOptions options)
        {
            _ = options.WhenNotNull();

            // Re-create the breadcrumbs if called more than once
            _breadcrumbs = new Breadcrumbs(options);

            return _breadcrumbs;
        }

        /// <summary>Clears all breadcrumb data and releases the global <see cref="Breadcrumbs"/> instance.</summary>
        public static void Destroy()
        {
            _breadcrumbs?.Clear();
            _breadcrumbs = null;
        }
    }
}
