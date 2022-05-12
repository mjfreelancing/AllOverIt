namespace AllOverIt.Diagnostics.Breadcrumbs
{
    /// <summary>Represents a factory that will create new <see cref="Breadcrumbs"/> instances.</summary>
    public interface IBreadcrumbsFactory
    {
        /// <summary>Creates a new <see cref="Breadcrumbs"/> instance.</summary>
        IBreadcrumbs CreateBreadcrumbs();
    }
}
