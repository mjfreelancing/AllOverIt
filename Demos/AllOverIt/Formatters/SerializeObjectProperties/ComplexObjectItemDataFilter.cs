using AllOverIt.Formatters.Objects;
using System.Collections.Generic;
using System.Linq;

namespace SerializeObjectProperties
{
    // The example using this filter configures the serializer to auto-collate the Path 'Items.Data.Values'
    internal sealed class ComplexObjectItemDataFilter : ObjectPropertyFilter, IRegisteredObjectPropertyFilter, IFormattableObjectPropertyFilter
    {
        private const int MaxItemCount = 3;

        public bool CanFilter(object @object)
        {
            return @object is ComplexObject;
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
            if (Parents.Any())
            {
                lastParent = Parents.Last();

                return lastParent.Name == nameof(ComplexObject.Item.ItemData.Values);
            }

            lastParent = null;
            return false;
        }
    }
}