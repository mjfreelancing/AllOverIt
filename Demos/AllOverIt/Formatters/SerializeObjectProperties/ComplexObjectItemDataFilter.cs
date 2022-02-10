using AllOverIt.Formatters.Objects;
using System.Collections.Generic;
using System.Linq;

namespace SerializeObjectProperties
{
    internal sealed class ComplexObjectItemDataFilter : ObjectPropertyFilter, IRegisteredObjectPropertyFilter, IFormattableObjectPropertyFilter
    {
        private const int MaxItemCount = 3;

        public bool CanFilter(object @object)
        {
            return @object is ComplexObject;
        }

        public override bool OnIncludeProperty()
        {
            EnumerableOptions.CollateValues = Parents.Any() &&
                                              Parents.Count >= 3 &&
                                              Parents.ElementAt(2).Name == "Data";

            return true;
        }

        public override bool OnIncludeValue()
        {
            // restrict the output of the 'Values' property to 3 values only
            return !AtValuesNode(out _) || Index < MaxItemCount;
        }

        public string OnFormatValue(string value)
        {
            // Reformat the values of ComplexObject.Item.ItemData if the max. limit is reached
            if (AtValuesNode(out var lastParent) && Index == MaxItemCount - 1)
            {
                var itemCount = ((IEnumerable<int>) lastParent.Value).Count();

                return $"{value} and {itemCount - MaxItemCount} additional values";
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