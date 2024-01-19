using AllOverIt.Formatters.Objects;

namespace SerializeObjectPropertiesDemo
{
    internal sealed class ComplexObjectFilter : ObjectPropertyFilter, IFormattableObjectPropertyFilter
    {
        public override bool OnIncludeProperty()
        {
            return Name != nameof(ComplexObject.ComplexItem.ComplexItemData.Values);
        }

        public string OnFormatValue(string value)
        {
            return Name == nameof(ComplexObject.ComplexItem.ComplexItemData.Timestamp)
                ? $"[{value}]"
                : value;
        }
    }
}