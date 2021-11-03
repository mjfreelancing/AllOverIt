using AllOverIt.Formatters.Objects;

namespace SerializeObjectProperties
{
    internal sealed class ComplexObjectFilter : ObjectPropertyFilter, IFormattableObjectPropertyFilter
    {
        public override bool OnIncludeProperty()
        {
            return Name != nameof(ComplexObject.Item.ItemData.Values);
        }

        public string OnFormatValue(string value)
{
            return Name == nameof(ComplexObject.Item.ItemData.Timestamp)
                ? $"[{value}]"
                : value;
        }
    }
}