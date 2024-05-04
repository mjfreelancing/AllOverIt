using AllOverIt.Formatters.Objects;

namespace SerializeObjectPropertiesDemo
{
    // The example using this filter configures the serializer to auto-collate the Path 'Items.Data.Values'
    internal sealed class ComplexObjectItemDataFilter : ObjectPropertyFilter, IFormattableObjectPropertyFilter
    {
        private const int MaxItemCount = 3;

        public ComplexObjectItemDataFilter()
        {
            // A filter's EnumerableOptions overrides the serializer global options.
            // The demo explicitly sets a path that will be auto-collated. If that was not provided then setting CollateValues
            // to true here will achieve the same outcome.
            //
            //EnumerableOptions.CollateValues = true;
            EnumerableOptions.Separator = ",";
        }

        public override bool OnIncludeValue()
        {
            // restrict the output of the 'Values' property to 3 values only
            return !AtValuesNode(out _) || Index < MaxItemCount;
        }

        public string OnFormatValue(string value)
        {
            // Reformat the remainder of the values
            if (AtValuesNode(out var lastParent) && Index == MaxItemCount - 1)
            {
                var itemCount = ((IEnumerable<int>) lastParent.Value).Count();
                var remainder = itemCount - MaxItemCount;

                return remainder == 0
                    ? value
                    : $"{value} and {remainder} additional values";
            }

            return value;
        }

        private bool AtValuesNode(out ObjectPropertyParent lastParent)
        {
            if (Parents.Count != 0)
            {
                lastParent = Parents.Last();

                return lastParent.Name == nameof(ComplexObject.ComplexItem.ComplexItemData.Values);
            }

            lastParent = null;
            return false;
        }
    }
}