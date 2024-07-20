using AllOverIt.Assertion;

namespace AllOverIt.Diagnostics.Breadcrumbs
{
    /// <inheritdoc cref="IBreadcrumbsFactory" />
    public sealed class BreadcrumbsFactory : IBreadcrumbsFactory
    {
        private readonly BreadcrumbsOptions _options;

        /// <summary>Initializes the factory with a default constructed <see cref="BreadcrumbsOptions"/> that will be used to construct
        /// new <see cref="Breadcrumbs"/> instances.</summary>
        public BreadcrumbsFactory()
            : this(new BreadcrumbsOptions())
        {
        }

        /// <summary>Initializes the factory with options that will be used to construct new <see cref="Breadcrumbs"/> instances.</summary>
        /// <param name="options">Options that control how breadcrumb items are inserted and cached by <see cref="Breadcrumbs"/>.</param>
        public BreadcrumbsFactory(BreadcrumbsOptions options)
        {
            _options = options.WhenNotNull();
        }

        /// <summary>Creates a new <see cref="Breadcrumbs"/> instance using the options initially provided to the factory.</summary>
        /// <returns></returns>
        public IBreadcrumbs CreateBreadcrumbs()
        {
            return new Breadcrumbs(_options);
        }
    }
}
