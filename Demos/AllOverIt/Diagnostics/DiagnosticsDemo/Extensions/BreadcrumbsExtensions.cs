using AllOverIt.Diagnostics.Breadcrumbs;

namespace DiagnosticsDemo.Extensions
{
    internal static class BreadcrumbsExtensions
    {
        public static IEnumerable<BreadcrumbData> WithRandomDataCollated(this IBreadcrumbs breadcrumbs)
        {
            return new RandomDataCollator(breadcrumbs, (current, next) =>
            {
                if (current is null)
                {
                    return next;        // int[]
                }

                var currentData = (int[]) current;
                var nextData = (int[]) next;

                int[] collated = [.. currentData, .. nextData];

                return collated;
            });
        }
    }
}