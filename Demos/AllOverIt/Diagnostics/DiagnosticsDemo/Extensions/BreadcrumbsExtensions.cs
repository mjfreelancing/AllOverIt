using AllOverIt.Diagnostics.Breadcrumbs;
using AllOverIt.Diagnostics.Breadcrumbs.Extensions;
using AllOverIt.Extensions;

namespace DiagnosticsDemo.Extensions
{
    internal static class BreadcrumbsExtensions
    {
        public static void AddIntArrayData(this IBreadcrumbs breadcrumbs, bool sequential)
        {
            var counter = 0;

            for (var i = 0; i < 10; i++)
            {
                var data = Enumerable
                    .Range(0, 5)
                    .SelectToArray(_ =>
                    {
                        counter++;

                        return sequential ? counter : Random.Shared.Next(100);
                    });

                breadcrumbs
                    .Add("Random Data", data)
                    .WithTag(App.RandomTag);
            }
        }

        public static IEnumerable<BreadcrumbData> WithRandomDataCollated(this IBreadcrumbs breadcrumbs, string tag)
        {
            // Iterate the data, collating consecutive breadcrumbs int[] data that have the same Tag
            var enumerator = breadcrumbs.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                yield break;
            }

            BreadcrumbData collated = null;
            int[] collatedData = null;

            while (true)
            {
                var current = enumerator.Current;

                var hasTag = current?.Tags?.Contains(tag) ?? false;

                if (hasTag)
                {
                    UpdateCollatedData(current, ref collated, ref collatedData);
                }
                else
                {
                    yield return GetNextToYield(current, collated, collatedData);

                    current = null;
                    collated = null;
                    collatedData = null;
                }

                if (!enumerator.MoveNext())
                {
                    if (current is not null)
                    {
                        yield return GetNextToYield(current, collated, collatedData);
                    }

                    yield break;
                }
            }
        }

        private static BreadcrumbData GetNextToYield(BreadcrumbData current, BreadcrumbData collated, int[] collatedData)
        {
            if (collated is null)
            {
                return current;
            }
            else
            {
                collated.AsMutable().WithMetadata(collatedData);

                return collated;
            }
        }

        private static void UpdateCollatedData(BreadcrumbData current, ref BreadcrumbData collated, ref int[] collatedData)
        {
            if (collated is null)
            {
                collated = current;
                collatedData = (int[]) current.Metadata;
            }
            else
            {
                collatedData = [.. collatedData, .. (int[]) current.Metadata];
            }
        }
    }
}
