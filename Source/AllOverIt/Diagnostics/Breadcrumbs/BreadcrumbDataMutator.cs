using AllOverIt.Assertion;

namespace AllOverIt.Diagnostics.Breadcrumbs
{
    internal sealed class BreadcrumbDataMutator : IBreadcrumbDataMutator
    {
        private readonly BreadcrumbData _breadcrumbData;

        public BreadcrumbDataMutator(BreadcrumbData breadcrumbData)
        {
            _breadcrumbData = breadcrumbData.WhenNotNull(nameof(breadcrumbData));
        }

        IBreadcrumbDataMutator IBreadcrumbDataMutator.WithMetadata(object metadata)
        {
            _breadcrumbData._metadata = metadata;

            return this;
        }

        IBreadcrumbDataMutator IBreadcrumbDataMutator.WithTag(string tag)
        {
            _breadcrumbData._tags = tag is null ? null : [tag];

            return this;
        }

        IBreadcrumbDataMutator IBreadcrumbDataMutator.WithTags(params string[] tags)
        {
            _breadcrumbData._tags = tags is null ? null : tags;

            return this;
        }
    }
}
